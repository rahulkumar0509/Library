using Library.Domain;

namespace Library.Repository
{
    public interface IMemberRepository
    {
        public int AddMember(Member member);
        public IEnumerable<Member> GetMembers();
    }
}