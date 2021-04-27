using System.Threading.Tasks;
using Connecterra.StateTracking.Common.Routing;
using Connecterra.StateTracking.MicroService.Command;
using Connecterra.StateTracking.MicroService.Query;
using Microsoft.AspNetCore.Mvc;

namespace Connecterra.StateTracking.MicroService.Controller
{
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly IRouter Router;

        public SensorController(IRouter router)
        {
            Router = router;
        }

        [Route("{yearToQuery}")]
        [HttpGet]
        public async Task<IActionResult> GetNewSensor(int yearToQuery)
        {
            var result = await Router.ExecuteAsync<NewSensorDeployedByYearQuery, int>(new NewSensorDeployedByYearQuery(yearToQuery));
            if (result.IsSuccess)
                return Ok(result.Value);
            return Problem(result.ErrorMessage);
        }

        [Route("{monthToQuery}/{yearToQuery}")]
        [HttpGet]
        public async Task<IActionResult> Get(int monthToQuery, int yearToQuery)
        {
            var result = await Router.ExecuteAsync<SensorDiedByMonthQuery, int>(new SensorDiedByMonthQuery(monthToQuery, yearToQuery));
            if (result.IsSuccess)
                return Ok(result.Value);
            return Problem(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSensorCommand createSensorCommand)
        {
            var result = await Router.ExecuteAsync(createSensorCommand);
            if (result.IsSuccess)
                return Ok();
            return Problem(result.ErrorMessage);
        }
        
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSensorCommand updateSensorCommand)
        {
            var result = await Router.ExecuteAsync(updateSensorCommand);
            if (result.IsSuccess)
                return Ok();
            return Problem(result.ErrorMessage);
        }

    }
}
