using AngularSecurity.Api.EntityClasses;
using AngularSecurity.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AngularSecurity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : AppControllerBase
    {
        private readonly ILogger<CategoryController> logger;
        private readonly PtcDbContext context;

        public CategoryController(ILogger<CategoryController> logger, PtcDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            IActionResult ret = null;
            List<Category> list = new List<Category>();

            try
            {
                if (this.context.Categories.Count() > 0)
                {
                    list = this.context.Categories.OrderBy(p => p.CategoryName).ToList();
                    ret = StatusCode(StatusCodes.Status200OK, list);
                }
                else
                {
                    ret = StatusCode(StatusCodes.Status404NotFound, "No Categories exist in the system.");
                }
            }
            catch (Exception ex)
            {
                ret = HandleException(ex, "Exception trying to get all categories");
            }

            return ret;
        }
    }
}
