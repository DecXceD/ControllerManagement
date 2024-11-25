using ControllerManagement.Data;
using ControllerManagement.Service;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ControllerManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerController : ControllerBase
    {
        private IControllerService service;

        public ControllerController(IControllerService service)
        {
            this.service = service;
        }

        // GET: api/<ControllerController>
        [Route("ShowAllControllers")]
        [HttpGet]
        public IEnumerable<string> ShowAllControllers()
        {
            List<string> allControllers = service.ShowAllControllers();
            return allControllers;
        }

        // GET api/<ControllerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ControllerController>
        [Route("AddController")]
        [HttpPost]
        public void AddController()
        {
            service.AddController();
        }

        // PUT api/<ControllerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ControllerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
