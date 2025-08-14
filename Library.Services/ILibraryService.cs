using Library.Domain;
namespace Library.Services{
    public interface ILibraryService
    {
        IEnumerable<Book> GetBooks();
        int AddBook(Book book);
    }   
}