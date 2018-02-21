using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using acquizapi.Models;

namespace acquizapi.Controllers
{
    [Produces("application/json")]
    [Route("api/ChineseChessAI")]
    public class ChineseChessAIController : Controller
    {
        // POST: api/ChineseChessAI
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/ChineseChessAI/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]ChineseChessAIInput value)
        {
            var output = new ChineseChessAIOutput();

            var setting = new Newtonsoft.Json.JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd",
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            return new JsonResult(output, setting);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
