using Library.Domain;
namespace Library.Services{
    public interface ILibraryService
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task AddBookAsync(Book book);
    }   
}