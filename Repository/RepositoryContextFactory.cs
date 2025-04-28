//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;

//namespace Repository;

//public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
//{
//    public RepositoryContext CreateDbContext(string[] args)
//    {
//        var configuration = new ConfigurationBuilder()
//            .SetBasePath(Directory.GetCurrentDirectory())
//            .AddJsonFile("appsettings.json")
//            .Build();

//        var builder = new DbContextOptionsBuilder<RepositoryContext>()
//            .UseSqlServer(
//                configuration.GetConnectionString("DefaultConnection"),
//                b => b.MigrationsAssembly("Comments.Server"));

//        return new RepositoryContext(builder.Options);
//    }
//}
