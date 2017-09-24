using IDDD.App.Cqs.Queries.Users;
using IDDD.Core.Cqs.Command;
using IDDD.Core.Cqs.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IDDD.Web.Api.Controllers
{
    public class UserInfoController: BaseController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandDispatcher _commandDispatcher;

        public UserInfoController(IQueryProcessor queryProcessor, ICommandDispatcher commandDispatcher)
        {
            if (queryProcessor == null) throw new ArgumentNullException(nameof(queryProcessor));
            if (commandDispatcher == null) throw new ArgumentNullException(nameof(commandDispatcher));
            _queryProcessor = queryProcessor;
            _commandDispatcher = commandDispatcher;
        }


        [Authorize]
        [HttpGet("~/api/userinfo")]
        public async Task<IActionResult> Get()
        {
            var identifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(identifier))
            {
                return BadRequest();
            }

            var query = new UserInfoQuery(Guid.Parse(identifier));
            var userInfo = await _queryProcessor.ProcessAsync(query);
            
            return OkResult(userInfo);
        }
    }
}
