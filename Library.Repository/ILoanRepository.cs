using Library.Domain;
using Library.Domain.Dto;

namespace Library.Repository
{
    public interface ILoanRepository
    {
        public int BorrowBook(Loan loan);
        public int ReturnBook(int MemberId, int BookId);
        public IEnumerable<Loan> GetTransactionByBook(string ISBN, string Title);
    }
}