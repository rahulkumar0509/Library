using System.Data;
using Dapper;
using Library.Domain;
using Microsoft.Data.SqlClient;

namespace Library.Repository.Impl
{
    public class MemberRepository : IMemberRepository
    {
        private string _connectionString;
        ILogger<MemberRepository> _logger;
        private LibraryDbContext _context;
        public MemberRepository(IConfiguration configuration, LibraryDbContext context, ILogger<MemberRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
            _context = context;
            _logger = logger;
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
            Console.WriteLine($"Email: {email}");
            var sql = "SELECT * FROM LibrarySchema.Members WHERE Email = @email";
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.QueryFirstOrDefault<Member>(sql, new { email }); // Use QueryFirstOrDefault instead of Query or QueryFirst in case of WHERE clause;
            }
        }

        public IEnumerable<Member> GetMembers(DateOnly date)
        {
            var x = date.ToDateTime(TimeOnly.MinValue); // covert dateonly to datetime
            _logger.LogInformation($"Registration Date: {x} and date: {date}");
            // match sql server table date time value and c# datetime value
            var y = _context.Members.Where(prop => prop.RegistrationDate.Date == x).ToList(); // .Date is to convert into Date
            _context.SaveChanges();
            return y;
        }
    }
}