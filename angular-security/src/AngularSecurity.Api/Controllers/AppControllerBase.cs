using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AngularSecurity.Api.Controllers
{
    public class AppControllerBase: ControllerBase
    {
        protected void SimulateLongRunning()
        {
            System.Threading.Thread.Sleep(1500);
        }

        protected IActionResult HandleException(Exception exception, string message)
        {
            IActionResult result;
            result = StatusCode(StatusCodes.Status500InternalServerError, new Exception(message, exception));
            return result;
        }
    }
}
