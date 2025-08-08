public class LibraryService: ILibraryService{
    private readonly List<Book> _books = new();

    public Task<IEnumerable<Book>> GetBooksAsync()
    {
        return Task.FromResult<IEnumerable<Book>>(_books);
    }

    public Task<Book?> GetBookByIdAsync(int id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        return Task.FromResult(book);
    }

    public Task AddBookAsync(Book book)
    {
        _books.Add(book);
        return Task.CompletedTask;
    }

    public Task UpdateBookAsync(Book book)
    {
        var existingBook = _books.FirstOrDefault(b => b.Id == book.Id);
        if (existingBook != null)
        {
            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.PublishedYear = book.PublishedYear;
        }
        return Task.CompletedTask;
    }

    public Task DeleteBookAsync(int id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book != null)
        {
            _books.Remove(book);
        }
        return Task.CompletedTask;
    }

}