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

namespace EmployeeManagement.Controllers
{
    public class UserController : Controller
    {
            private readonly IConfiguration _configuration;
        private readonly UserRepository _userRepository;
        public UserController(IConfiguration configuration,UserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }
        [HttpPost]
        [Route("login")]

        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (login == null)
            {
                return BadRequest();
            }


            var user = this._userRepository.GetUserByEmail(login.Email);

            if (user == null)
            {

                return NotFound();
            }

            if(user.Password != login.Password)
            {
                return BadRequest("Invalid password");
            }

            var token = CreateToken(user);

            return Ok(new { token = token });


        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

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
        public ActionResult<List<User>> GetAllUsers()
        {
            try
            {
                List<User> UsersList = this._userRepository.GetAllUsers();
                return Ok(UsersList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpPost]
        [Route("add-user")]
        [ Authorize(Roles = "Admin")]
        public ActionResult AddUser([FromBody] User user)
        {
            try
            {
                Console.WriteLine(user.Email);
                this._userRepository.RegisterUser(user);
                return Ok(user);
            }
            catch (Exception ex)
            {   
                Console.WriteLine(ex.ToString());
                return BadRequest(ex.Message.ToString());
            }
        }
        [HttpGet]
        [Route("get-user/{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            try
            {
                User userindb = this._userRepository.GetUserById(id);

                if(userindb == null) {
                    return BadRequest("user not found");
                }
                return Ok(userindb);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpPut]
        [Route("update-user"), Authorize(Roles = "Admin")]
        public ActionResult UpdateUser([FromBody] User emp)
        {
            try
            {
                 this._userRepository.UpdateUser(emp);
               
                return Ok("User updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpDelete]
        [Route("delete-user/{id}"), Authorize(Roles = "Admin")]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                this._userRepository.DeleteUser(id);
               
                return Ok("User Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

    }

}
