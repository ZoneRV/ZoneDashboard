using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ZoneProductionLibrary.ProductionServices.Base;

namespace ZoneProductionDashBoard.Controllers
{
    [ApiController, Microsoft.AspNetCore.Mvc.Route("api/[controller]/")]
    public class ChecksController : Controller
    {
        private IProductionService _productionService;

        private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            Converters = [new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() }], 
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };
        
        public ChecksController(IProductionService productionService)
        {
            _productionService = productionService;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            var check = _productionService.GetCheck(id);

            if (check is not null)
                return Ok(JsonConvert.SerializeObject(check, _serializerSettings));

            else
                return NotFound($"Check with id {id} not found");
        }
    }
}