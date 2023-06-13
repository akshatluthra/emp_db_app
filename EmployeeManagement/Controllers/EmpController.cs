using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using EmployeeManagement.Repositories;
using Emp_DatabaseApp.Custom_Action_Filter;

namespace EmployeeManagement.Controllers
{
    public class EmpController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly EmpRepository empRepository;

        public EmpController(IConfiguration configuration,EmpRepository empRepository)
        {
            this.configuration = configuration;
            this.empRepository = empRepository;
        }


        [HttpPost]
        [Route("login")]

        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (login == null)
            {
                return BadRequest();
            }


            var employee = empRepository.GetEmployee(login.Email);

            if (employee == null)
            {

                return NotFound();
            }

            if(employee.Password != login.Password)
            {
                return BadRequest("Invalid password");
            }

            var token = CreateToken(employee);

            return Ok(new { token = token });


        }
        private string CreateToken(Employee employee)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.Email),
                new Claim(ClaimTypes.Role, employee.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        [HttpGet]
        [Route("get-all")]
        [Authorize()]
        [ValidateModel]
        public ActionResult<List<Employee>> GetAll()
        {
            List<Employee> employeeData = empRepository.GetEmployees();
            return Ok(employeeData);
        }

        [HttpPost]
        [Route("add-employee")]
        [ Authorize(Roles = "Admin")]
        [ValidateModel]
        public ActionResult Create([FromBody] Employee employee)
        {
            Console.WriteLine(employee.Email);
            this.empRepository.AddEmployee(employee);
            return Ok(employee);
        }


        [HttpGet]
        [Route("get-employee/{id}")]
        [ValidateModel]
        public ActionResult<Employee> GetById(int id)
        {
            Employee employeeDb = this.empRepository.GetById(id);

            if(employeeDb == null) {
                return BadRequest("employee with certain Id is not found");
            }
            return Ok(employeeDb);
        }

        [HttpPut]
        [Route("update-employee"), Authorize(Roles = "Admin")]
        [ValidateModel]
        public ActionResult Update([FromBody] Employee emp)
        {
            
            this.empRepository.UpdateEmp(emp);
               
            return Ok("Employee Data has been Updated");
        }

        [HttpDelete]
        [Route("delete-employee/{id}"), Authorize(Roles = "Admin")]
        [ValidateModel]
        public ActionResult Delete(int id)
        {
            
            this.empRepository.DeleteEmp(id);
               
            return Ok("Employee Data has been deleted successfully");
            
        }

    }

}
