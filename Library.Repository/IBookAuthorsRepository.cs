namespace Library.Repository
{
    public interface IBookAuthorsRepository
    {
        public int AddBookAuthor(int bookId, int authorId);
    }
}