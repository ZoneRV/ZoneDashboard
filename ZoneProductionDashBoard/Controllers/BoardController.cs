using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ZoneProductionLibrary.ProductionServices.Base;

namespace ZoneProductionDashBoard.Controllers
{
    [ApiController, Microsoft.AspNetCore.Mvc.Route("api/[controller]/")]
    public class BoardsController : Controller
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
        
        public BoardsController(IProductionService productionService)
        {
            _productionService = productionService;
        }
        
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult> ById(string id)
        {
            var van = await _productionService.GetBoardByIdAsync(id);

            if (van is not null)
                return Ok(JsonConvert.SerializeObject(van, _serializerSettings));

            else
                return NotFound($"Van with id {id} not found");
        }
        
        [HttpGet("[action]/{name}")]
        public async Task<ActionResult> ByName(string name)
        {
            var van = await _productionService.GetBoardByNameAsync(name);

            if (van is not null)
                return Ok(JsonConvert.SerializeObject(van, _serializerSettings));

            else
                return NotFound($"Van with name {name} not found");
        }
    }
}