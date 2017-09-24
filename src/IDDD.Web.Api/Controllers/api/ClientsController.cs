using Microsoft.AspNetCore.Mvc;
using IDDD.Core.Cqs.Command;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace IDDD.Web.Api.Controllers.api
{
    [Route("api/[controller]")]
    public class ClientsController : BaseController
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public ClientsController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

    }
}
