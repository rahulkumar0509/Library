using Library.Domain;

namespace Library.Repository
{
    public interface IAuthorRepository
    {
        public int AddAuthor(Author author);
        public IEnumerable<Author> GetAuthors();
    }
}