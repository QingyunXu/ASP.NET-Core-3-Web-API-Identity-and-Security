# ASP.NET Core 3 Web API Identity and Security

## Setting up the User Store (Database)

- NuGet Packages

```sh
dotnet add package Microsoft.AspNetCore.Identity
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet tool install --global dotnet-ef
```

- Migrations and Database

```sh
dotnet ef migrations add Init
dotnet ef database update
```

## Authentication

- NugGet Packages

```
dotnet add package Microsoft.IdentityModel.Tokens
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```
