using System.Data;
using Microsoft.EntityFrameworkCore;
using RevaliInstruct.Core.Data;

namespace RevaliInstruct.Api.Middleware
{
    public class SqlSessionContextMiddleware
    {
        private readonly RequestDelegate _next;

        public SqlSessionContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
        {
            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                             ?? context.User.FindFirst("sub")?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                try
                {
                    var conn = db.Database.GetDbConnection();
                    if (conn.State != ConnectionState.Open) await conn.OpenAsync();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "EXEC sys.sp_set_session_context @key=N'UserId', @value=@p0;";
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@p0";
                        p.Value = userId;
                        cmd.Parameters.Add(p);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    var isAdmin = context.User.IsInRole("Admin") ? "true" : "false";
                    using (var cmd2 = conn.CreateCommand())
                    {
                        cmd2.CommandText = "EXEC sys.sp_set_session_context @key=N'IsAdmin', @value=@p0;";
                        var p2 = cmd2.CreateParameter();
                        p2.ParameterName = "@p0";
                        p2.Value = isAdmin;
                        cmd2.Parameters.Add(p2);
                        await cmd2.ExecuteNonQueryAsync();
                    }
                }
                catch
                {
                    // Fout bij instellen session context negeren
                }
            }

            await _next(context);
        }
    }
}
