dotnet ef dbcontext scaffold "Server=tcp:dalapagos-db-server.database.windows.net,1433;Initial Catalog=DalapagosTunnelsDb;Persist Security Info=False;User ID=TunnelsAdmin;Password=Map0tofu42;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" Microsoft.EntityFrameworkCore.SqlServer --force --schema dbo