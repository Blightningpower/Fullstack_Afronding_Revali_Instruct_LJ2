# -----------------------------
# BUILD STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Kopieer solution + backend
COPY ./RevaliSolution/RevaliSolution.sln ./RevaliSolution.sln
COPY ./backend ./backend

# Restore, Build, en Publish
RUN dotnet restore ./backend/RevaliInstruct.Api/RevaliInstruct.Api.csproj
RUN dotnet publish ./backend/RevaliInstruct.Api/RevaliInstruct.Api.csproj -c Release -o /src/out

# -----------------------------
# RUNTIME STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Stel ASP.NET Core poort in
ENV ASPNETCORE_URLS=http://+:80

# Kopieer gepubliceerde output vanuit build-stage
COPY --from=build /src/out .

# Start de API
ENTRYPOINT ["dotnet", "RevaliInstruct.Api.dll"]