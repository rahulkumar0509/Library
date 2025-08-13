using System.Data;
using Dapper;
using Library.Domain;
using Microsoft.Data.SqlClient;
namespace Library.Repository.Impl{
    public class BookRepository: IBookRepository {
        private string _connectionString;
        public BookRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "";
        }
        public Task AddBookAsync(Book book)
        {
            return null;
        }
        public Task DeleteBookAsync(int id)
        {
            return null;
        }
        public Task FetchBook()
        {
            var sql = "SELECT * FROM LibrarySchema.Book";
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var data = connection.Query(sql);
                Console.WriteLine(data);
            }
            return null;
        }
    }
}