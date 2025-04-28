using ControllerManagement.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ControllerManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerController : ControllerBase
    {
        private readonly IControllerService service;

        public ControllerController(IControllerService service)
        {
            this.service = service;
        }

        // GET: api/<ControllerController>
        [Route("ShowAllControllers")]
        [HttpGet]
        public IEnumerable<string> ShowAllControllers()
        {
            return service.ShowAllControllers();
        }

        // GET api/<ControllerController>/5
        [HttpGet("{id}")]
        public ActionResult<Data.Controller> GetController(int id)
        {
            try
            {
                return service.GetController(id);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST api/<ControllerController>
        [Route("AddController")]
        [HttpPost]
        //[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public int AddController()
        {
            return service.AddController();
        }

        [Route("AddParameter/{id}")]
        [HttpPatch]
        //[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public ActionResult AddParameter(int id, string name, [FromBody]Parameter value)
        {
            try
            {
                service.AddParameter(id, name, value);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE api/<ControllerController>/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public ActionResult DeleteController(int id)
        {
            try
            {
                service.DeleteController(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Route("DeleteParameter/{id}")]
        [HttpPatch]
        //[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public ActionResult DeleteParameter(int id, string name)
        {
            try
            {
                service.DeleteParameter(id, name);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Route("UpdateParameter/{id}")]
        [HttpPatch]
        //[Authorize(Roles = "Admin, Worker", AuthenticationSchemes = "Bearer")]
        public ActionResult UpdateParameter(int id, string name, double value)
        {
            try
            {
                service.UpdateParameter(id, name, value);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Route("RenameParameter/{id}")]
        [HttpPatch]
        //[Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public ActionResult RenameParameter(int id, string name, string newName)
        {
            try
            {
                service.RenameParameter(id, name, newName);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
