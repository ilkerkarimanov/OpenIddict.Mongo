using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IDDD.Core.Cqs.Query;
using IDDD.Core.Cqs.Command;
using IDDD.App.Cqs.Commands.Users;
using IDDD.Core;

namespace IDDD.Web.Api.Controllers.Api
{
    public class AccountController : BaseController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandDispatcher _commandDispatcher;

        public AccountController(IQueryProcessor queryProcessor, ICommandDispatcher commandDispatcher)
        {
            if (queryProcessor == null) throw new ArgumentNullException(nameof(queryProcessor));
            if (commandDispatcher == null) throw new ArgumentNullException(nameof(commandDispatcher));
            _queryProcessor = queryProcessor;
            _commandDispatcher = commandDispatcher;
        }



        [AllowAnonymous]
        [Route("~/api/account/register")]
        public async Task<IActionResult> Register([FromBody]RegistrationModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return ErrorModelResult();
            }
            try
            {
                var command = new RegisterUserCommand(userModel.Email, userModel.ClientKey);
                var result = await _commandDispatcher.DispatchAsync<RegisterUserCommand,Result>(command);
                return ToResult(result);
            }catch(FailureResult reqEx)
            {
                return ErrorResult(reqEx);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("~/api/account/confirmemail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand command)
        {
            if (!ModelState.IsValid)
            {
                return ErrorModelResult();
            }
            try {
                var result = await _commandDispatcher.DispatchAsync<ConfirmEmailCommand,Result>(command);
                return ToResult(result);
            }
            catch(FailureResult reqEx)
            {
                return ErrorResult(reqEx);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("~/api/account/password")]
        public async Task<IActionResult> CreatePasword([FromBody] CreatePasswordCommand command)
        {
            if (!ModelState.IsValid)
            {
                return ErrorModelResult();
            }
            try
            {
                var result = await _commandDispatcher.DispatchAsync<CreatePasswordCommand,Result>(command);
                return ToResult(result);
            }
            catch (FailureResult reqEx)
            {
                return ErrorResult(reqEx);
            }
        }


        [HttpPost]
        [Route("~/api/account/forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            if (!ModelState.IsValid)
            {
                return ErrorModelResult();
            }
            try
            {
                var result = await _commandDispatcher.DispatchAsync<ForgotPasswordCommand, Result>(command);
                return ToResult(result);
            }
            catch (FailureResult reqEx)
            {
                return ErrorResult(reqEx);
            }
        }

        [HttpPost]
        [Route("~/api/account/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            if (!ModelState.IsValid)
            {
                return ErrorModelResult();
            }
            try
            {
                var result = await _commandDispatcher.DispatchAsync<ResetPasswordCommand, Result>(command);
                return ToResult(result);
            }
            catch (FailureResult reqEx)
            {
                return ErrorResult(reqEx);
            }
        }
    }
}
