using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Library.Repository.Impl
{
    public class BookAuthorsRepository : IBookAuthorsRepository
    {
        private string _connectionString;
        public BookAuthorsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }
        public int AddBookAuthor(int bookId, int authorId)
        {
            var sql = "INSERT INTO LibrarySchema.BookAuthors (BookId, AuthorId) VALUES(@BookId, @AuthorId)";
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Execute(sql, new { BookId = bookId, AuthorId = authorId });
            }
        }
    }
}