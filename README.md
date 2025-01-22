## User Secrets Configuration Guide

Configure sensitive information using .NET User Secrets by following these steps.

## Steps

### 1. Initialize User Secrets
Run this command in the project directory (where your `.csproj` file is located):
```bash
dotnet user-secrets init
```


### 2. Add Connection String
Store your connection string securely by running:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=<Server>;Database=<Database>;User Id=<User>;Password=<Password>;TrustServerCertificate=True;"
```
Please replace `<Server>`, `<Database>`, `<User>`, and `<Password>` with your actual database values.
