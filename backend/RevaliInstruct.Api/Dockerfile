FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ./RevaliSolution/RevaliSolution.sln ./RevaliSolution.sln
COPY ./backend ./backend

RUN dotnet restore ./backend/RevaliInstruct.Api/RevaliInstruct.Api.csproj
RUN dotnet publish ./backend/RevaliInstruct.Api/RevaliInstruct.Api.csproj -c Release -o /src/out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "RevaliInstruct.Api.dll"]