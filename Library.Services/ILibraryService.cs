public class ILibraryService
{
    Task<IEnumerable<Book>> GetBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task AddBookAsync(Book book);
    Task UpdateBookAsync(Book book);
    Task DeleteBookAsync(int id);
}   