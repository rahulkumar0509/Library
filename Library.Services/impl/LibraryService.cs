using Library.Domain;
using Library.Repository;
namespace Library.Services.Impl
{
    public class LibraryService : ILibraryService
    {
        IBookRepository _bookRepository;
        public LibraryService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public IEnumerable<Book> GetBooks()
        {
            return _bookRepository.FetchBook();
        }
        public int AddBook(Book book)
        {
            return _bookRepository.AddBook(book);
        }
    }
}