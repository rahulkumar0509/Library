using System.ComponentModel.DataAnnotations;

namespace Library.Domain
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; } // define as public so it can be used in other class; which means same model can be used as payload for endpoint
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Biography { get; set; }
    }
}