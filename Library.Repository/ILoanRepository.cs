using Library.Domain;
using Library.Domain.Dto;

namespace Library.Repository
{
    public interface ILoanRepository
    {
        public int BorrowBook(Loan loan);
    }
}