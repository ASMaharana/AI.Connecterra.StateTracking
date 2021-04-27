using System;
using System.Threading.Tasks;
using Connecterra.StateTracking.Common.Routing;
using Connecterra.StateTracking.MicroService.Command;
using Connecterra.StateTracking.MicroService.Query;
using Microsoft.AspNetCore.Mvc;

namespace Connecterra.StateTracking.MicroService.Controller
{
    [Route("api/[controller]")]
    public class CowController : ControllerBase
    {
        private readonly IRouter Router;

        public CowController(IRouter router)
        {
            Router = router;
        }

        [Route("{dateToQuery}")]
        [HttpGet]
        public async Task<IActionResult> Get(DateTime dateToQuery)
        {
            var result = await Router.ExecuteAsync<GetNumberOfCowPregnantQuery, int>(new GetNumberOfCowPregnantQuery(dateToQuery));
            if (result.IsSuccess)
                return Ok(result.Value);
            return Problem(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCowCommand createCowCommand)
        {
            var result = await Router.ExecuteAsync(createCowCommand);
            if (result.IsSuccess)
                return Ok();
            return Problem(result.ErrorMessage);
        }
        
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCowCommand updateCowCommand)
        {
            var result = await Router.ExecuteAsync(updateCowCommand);
            if (result.IsSuccess)
                return Ok();
            return Problem(result.ErrorMessage);
        }

    }
}
