using SistFormAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SistFormAPI.Data
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

        public DbSet<CFormElement> FormElements { get; set; }

        public DbSet<CFormResponse> FormResponses { get; set; }
        public DbSet<CFormResponseDetail> FormResponseDetails { get; set; }
        public DbSet<FormAssignment> FormAssignments { get; set; }


        //Configuración del modelo de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuración de User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users"); // Nombre real de la tabla en MySQL (minúsculas)
                entity.HasIndex(u => u.UserName).IsUnique(); //Indice único
            });

            // Configuración de CForm
            modelBuilder.Entity<CForm>(entity =>
            {
                entity.ToTable("createforms");
                entity.HasKey(e => e.IdForm);
                entity.HasMany(e => e.Elements)
                      .WithOne(e => e.Form)
                      .HasForeignKey(e => e.FormId)
                      .HasConstraintName("form_elements_ibfk_1")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de CFormElement
            modelBuilder.Entity<CFormElement>(entity =>
            {
                entity.ToTable("form_elements");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
