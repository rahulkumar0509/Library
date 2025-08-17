using System.Data;
using Dapper;
using Library.Domain;
using Microsoft.Data.SqlClient;

namespace Library.Repository.Impl
{
    public class AuthorRepository : IAuthorRepository
    {
        private string _connectionString;
        public AuthorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }
        public int AddAuthor(Author author)
        {
            var sql = "INSERT INTO LibrarySchema.Authors (FirstName, LastName, Biography)"
                + " VALUES (@FirstName, @LastName, @Biography);"
                + "SELECT CAST(SCOPE_IDENTITY() AS INT)";

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.QuerySingle<int>(sql, author);
            }
        }
        public IEnumerable<Author> GetAuthors()
        {
            var sql = "SELECT * FROM LibrarySchema.Authors;";
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Author>(sql);
            }
        }
        public Author GetAuthorByName(string name)
        {
            // sql to find author by first + Last Name
            var sql = "SELECT * FROM LibrarySchema.Authors WHERE FirstName + ' ' + LastName = @name;";
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.QuerySingle<Author>(sql, new {name = name});
            }
        }
    }
}