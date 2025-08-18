using System.Data;
using Dapper;
using Library.Domain;
using Library.Domain.Dto;
using Microsoft.Data.SqlClient;

namespace Library.Repository.Impl
{
    public class LoanRepository : ILoanRepository
    {
        private string _connectionString;
        public LoanRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }
        public int BorrowBook(Loan loan)
        {
            var sql = "INSERT INTO LibrarySchema.Loans (BookId, MemberId, LoanDate, DueDate, isReturned) VALUES"
                + "(@BookId, @MemberId, @LoanDate, @DueDate, @isReturned)";

            Console.WriteLine(loan);
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Execute(sql, loan);
            }
        }
        public int ReturnBook(int MemberId, int BookId)
        {
            var sql = "UPDATE LibrarySchema.Loans SET isReturned = 1, ReturnDate = GETDATE() WHERE MemberId = @MemberId AND BookId = @BookId;";
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Execute(sql, new {MemberId, BookId});
            }
        }
    }
} 