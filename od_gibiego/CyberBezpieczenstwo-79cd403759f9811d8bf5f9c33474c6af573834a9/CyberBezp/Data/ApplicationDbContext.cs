using CyberBezp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CyberBezp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var assembly = typeof(CyberBezpContext).Assembly;
            var types = assembly.GetTypes()
                .Where(x => x.GetCustomAttribute<TableAttribute>() is not null)
                .ToList();

            foreach (var type in types)
            {
                var entity = builder.Entity(type);

                var tableName = type.GetCustomAttribute<TableAttribute>()!.Name;
                entity.ToTable(tableName);
            }
        }
    }
}
