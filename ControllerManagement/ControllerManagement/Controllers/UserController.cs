using ControllerManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace ControllerManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [Route("GetUsers")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetUsers()
        {
            return userService.GetUsers();
        }

        [Route("AddUser")]
        [HttpPost]
        public ActionResult AddUser(string username, string password)
        {
            try
            {
                userService.AddUser(username, password);
                return Created();
            }
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteUser")]
        [HttpDelete]
        public ActionResult DeleteUser(string username)
        {
            try
            {
                userService.DeleteUser(username);
                return Ok();
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("AddPersonToRole")]
        public  ActionResult AddPersonToRole(string username, string role)
        {
            try
            {
                userService.AddPersonToRole(username, role);
                return Ok();
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public ActionResult Login(string username, string password)
        {
            var user = this.userService.Authenticate(username, password);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect." });
            }

            var tokenString = this.userService.GenerateJSONWebToken(user);
            return Ok(new { token = tokenString });
        }
    }
}
