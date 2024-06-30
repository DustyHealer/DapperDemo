using Dapper;
using DapperDemo.Data;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperDemo.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private IDbConnection db;
        public CompanyRepository(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public Company Add(Company company)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES (@Name, @Address, @City, @State, @PostalCode);" + "SELECT CAST(SCOPE_IDENTITY() as int);";

            //var id = db.Query<int>(sql, new 
            //{ 
            //    @Name = company.Name, 
            //    @Address = company.Address, 
            //    @City = company.City, 
            //    @State = company.State, 
            //    @PostalCode = company.PostalCode 
            //}).Single();

            // Dapper can link parameters from the object automatically, since field names are same
            var id = db.Query<int>(sql, company).Single();
            company.CompanyId = id;
            return company;
        }

        public Company Find(int id)
        {
            var sql = "SELECT * FROM Companies WHERE CompanyId = @CompanyId";
            return db.Query<Company>(sql, new { @CompanyId = id }).Single();

        }

        public List<Company> GetAll()
        {
            var sql = "SELECT * FROM Companies";
            // We need to use db.query when the sql is returning something from the sql query
            return db.Query<Company>(sql).ToList();
        }

        public void Remove(int id)
        {
            var sql = "DELETE FROM Companies WHERE CompanyId = @CompanyId";
            db.Execute(sql, new { @CompanyId = id  });
        }

        public Company Update(Company company)
        {
            var sql = "UPDATE Companies SET Name = @Name, Address = @Address, City = @City, State = @State, PostalCode = @PostalCode WHERE CompanyId = @CompanyId";
            // SQL Parameter names and company model properties names should be exactly same
            // Execute is used when we do not need to retrieve anything from the database
            db.Execute(sql, company);
            return company;
        }
    }
}
