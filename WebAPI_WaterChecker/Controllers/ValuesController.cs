using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace WebAPI_WaterChecker.Controllers
{
    [Route("api/[controller]")]
    public class WatercheckerController : Controller
    {
        // GET: api/values
        [HttpGet]
        public api_water_counter[] Get(api_water_counter[] awc)
        {
            //awc[0].val_check = 1;
            return new api_water_counter[] { new api_water_counter(), new api_water_counter()};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
