using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ZoneProductionLibrary.ProductionServices.Base;

namespace ZoneProductionDashBoard.Controllers
{
    [ApiController, Microsoft.AspNetCore.Mvc.Route("api/[controller]/")]
    public class RedCardsController : Controller
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
        
        public RedCardsController(IProductionService productionService)
        {
            _productionService = productionService;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult> ById(string id)
        {
            var redCard = _productionService.GetRedCard(id);

            if (redCard is not null)
                return Ok(JsonConvert.SerializeObject(redCard, _serializerSettings));

            else
                return NotFound($"Red card with id {id} not found");
        }
    }
}