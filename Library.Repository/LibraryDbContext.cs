using Library.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Repository
{
    // define HasNoKey for table missing primary key
    // define table schema 
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> dbContextOptions) : base(dbContextOptions) // to get the configuration from program.cs which says to use SqlServer
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<BookAuthors> BookAuthors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlServer(_connectionString) 
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("LibrarySchema");
            modelBuilder.Entity<Book>(entity =>
            {
                 entity.Property(p => p.BookId)
                    .HasColumnName("falana")
                    .IsRequired()
                    .HasMaxLength(300);
            });
            modelBuilder.Entity<BookAuthors>(entity =>
            {
                entity.HasNoKey();
            });
        }

    }
}