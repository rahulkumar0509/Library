using System.Data;
using Dapper;
using Library.Domain;
using Microsoft.Data.SqlClient;

namespace Library.Repository.Impl
{
    public class MemberRepository : IMemberRepository
    {
        private string _connectionString;
        public MemberRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }
        public int AddMember(Member member)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO LibrarySchema.Members (FirstName, LastName, Email, RegistrationDate) VALUES (@FirstName, @LastName, @Email, @RegistrationDate); SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var memberId = connection.QuerySingle<int>(sql, member);
                Console.WriteLine(memberId);
                return memberId;
            }
        }

        public IEnumerable<Member> GetMembers()
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM LibrarySchema.Members;";
                return connection.Query<Member>(sql);
            }
        }

        public Member GetMemberByEmail(string email)
        {
            var sql = "SELECT [MemberId] FROM LibrarySchema.Members WHERE Email = @email";
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.QueryFirst<Member>(sql, new {email = email});
            }
        }
    }
}