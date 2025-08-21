using System.Data;
using Dapper;
using Library.Domain;
using Microsoft.Data.SqlClient;
namespace Library.Repository.Impl{
    public class BookRepository : IBookRepository
    {
        private string _connectionString;
        public BookRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }
        public int AddBook(Book book)
        {
            var sql = "INSERT INTO LibrarySchema.Books ("
             + "Title, ISBN, PublicationYear, Genre) VALUES ("
             + "@Title, @ISBN, @PublicationYear, @Genre);"
             + "SELECT CAST(SCOPE_IDENTITY() AS INT);"; // this one is to fetch latest Identity;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.QuerySingle<int>(sql, book);
            }
        }
        public Task DeleteBookAsync(int id)
        {
            return null;
        }
        public IEnumerable<Book> FetchBook()
        {
            var sql = "SELECT * FROM LibrarySchema.Books";
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var data = connection.Query<Book>(sql);
                return data;
            }
        }
        public Book GetBookByDetails(string ISBN, string Title)
        {
            var sql = "SELECT * FROM LibrarySchema.Books WHERE ISBN = @ISBN AND Title = @Title";
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.QuerySingle<Book>(sql, new { ISBN, Title });
            }
        }

        public void UpdateBookAvailability(int Copies, int BookId) // Task return type is only for Async operation
        {
            var sql = "UPDATE LibrarySchema.Books SET CopiesAvailable = @Copies WHERE BookId = @BookId";
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sql, new { Copies, BookId });
            }
        }
    }
}