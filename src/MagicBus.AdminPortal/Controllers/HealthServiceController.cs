using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicBus.AdminPortal.Application.HealthService;
using MagicBus.AdminPortal.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicBus.AdminPortal.Controllers
{
	public class HealthServiceController : ApiController
	{
		[HttpGet]
		public async Task<ActionResult<IEnumerable<HealthCheckInfo>>> GetHealthCheckInfo(CancellationToken ct)
		{
			return new OkObjectResult(await Mediator.Send(new GetHealthCheckInfo(), ct));
		}
	}
}
