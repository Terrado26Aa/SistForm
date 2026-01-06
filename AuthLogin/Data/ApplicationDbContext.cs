using AuthLogin.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AuthLogin.Data
{
    public class ApplicationDbContext : DbContext
    {
        //Constructor que recibe las opciones de configuración del DbContext
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        //Definición del DbSet para la entidad User
        public DbSet<User> Users { get; set; }

        //Definición del DbSet para la entidad CForm
        public DbSet<CForm> CForms { get; set; }

        //Configuración del modelo de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users"); //asegura el nombre de la tabla
                entity.HasIndex(u => u.UserName).IsUnique(); //Indice único
            });
        }
    }
}
