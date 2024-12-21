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
    }
}
