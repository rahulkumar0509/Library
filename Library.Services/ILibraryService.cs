using Library.Domain;
using Library.Domain.Dto;
namespace Library.Services{
    public interface ILibraryService
    {
        IEnumerable<Book> GetBooks();
        int AddBook(Book book);
        int AddAuthor(Author author);
        IEnumerable<Author> GetAuthors();
        // public int AddBookAuthor(int bookId, int authorId);
        public int AddBookAuthorDetails(BookInfo bookInfo);
        public int AddMember(Member member);
        public IEnumerable<Member> GetMembers();
        public int BorrowBook(BorrowBook book);
    }   
}