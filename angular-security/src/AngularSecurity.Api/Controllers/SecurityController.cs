using AngularSecurity.Api.EntityClasses;
using AngularSecurity.Api.ManagerClasses;
using AngularSecurity.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AngularSecurity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : AppControllerBase
    {
        private readonly PtcDbContext DbContext;
        private readonly JwtSettings settings;
        private readonly ILogger<ProductController> logger;
        public SecurityController(ILogger<ProductController> logger, PtcDbContext context, JwtSettings settings)
        {
            this.logger = logger;
            this.DbContext = context;
            this.settings = settings;
        }
        

        [HttpPost("Login")]
        public IActionResult Login([FromBody] AppUser user)
        {
            IActionResult ret = null;
            AppUserAuth auth = new AppUserAuth();
            SecurityManager mgr = new SecurityManager(DbContext, auth, settings);

            auth = (AppUserAuth)mgr.ValidateUser(user.UserName, user.Password);
            if (auth.IsAuthenticated)
            {
                ret = StatusCode(StatusCodes.Status200OK, auth);
            }
            else
            {
                ret = StatusCode(StatusCodes.Status404NotFound, "Invalid User Name/Password.");
            }

            return ret;
        }
    }
}
