using Library.Domain;
namespace Library.Services{
    public interface ILibraryService
    {
        IEnumerable<Book> GetBooks();
        int AddBook(Book book);
        int AddAuthor(Author author);
        IEnumerable<Author> GetAuthors();
        public int AddBookAuthor(int bookId, int authorId);
        public int AddMember(Member member);
        public IEnumerable<Member> GetMembers();
    }   
}