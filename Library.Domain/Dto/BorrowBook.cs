namespace Library.Domain.Dto
{
    public class BorrowBook
    {
        public required string BookTitle { get; set; }
        public required string BookIsbn { get; set; }
        public required string AuthorFullName { get; set; }
        public required int MemberId { get; set; }
        


    }
}