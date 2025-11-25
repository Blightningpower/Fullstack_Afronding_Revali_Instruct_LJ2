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
            // Voor nu: zet UserId = 0 en IsAdmin = false om compile errors weg te hebben.
            int? userId = 0;

            try
            {
                var conn = db.Database.GetDbConnection();
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "EXEC sys.sp_set_session_context @key=N'UserId', @value=@p0;";
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@p0";
                    p.Value = userId ?? (object)DBNull.Value;
                    cmd.Parameters.Add(p);
                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd2 = conn.CreateCommand())
                {
                    cmd2.CommandText = "EXEC sys.sp_set_session_context @key=N'IsAdmin', @value=@p0;";
                    var p2 = cmd2.CreateParameter();
                    p2.ParameterName = "@p0";
                    p2.Value = "false";
                    cmd2.Parameters.Add(p2);
                    await cmd2.ExecuteNonQueryAsync();
                }
            }
            catch
            {
                // ignore errors in dev
            }

            await _next(context);
        }
    }
}
