using System.ComponentModel.DataAnnotations;

namespace Library.Domain
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime RegistrationDate { get; set; }

        // This is the collection of loans for a specific member.
        // The InverseProperty annotation links this collection back to the 'Member' property on the Loan model.
        // [InverseProperty("Member")]
        // public ICollection<Loan> Loans { get; set; } = new List<Loan>(); 
    }
}