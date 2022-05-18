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
    [Authorize(Policy = "CanAccessProducts")]
    public class ProductController : AppControllerBase
    {
        private readonly ILogger<ProductController> logger;
        private readonly PtcDbContext context;

        public ProductController(ILogger<ProductController> logger, PtcDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IActionResult ret = null;
            List<Product> list = new List<Product>();

            // SimulateLongRunning();

            try
            {
                if (this.context.Products.Count() > 0)
                {
                    list = this.context.Products.OrderBy(p => p.ProductName).ToList();
                    ret = StatusCode(StatusCodes.Status200OK, list);
                }
                else
                {
                    ret = StatusCode(StatusCodes.Status404NotFound, "No Products exist in the system.");
                }
            }
            catch (Exception ex)
            {
                ret = HandleException(ex, "Exception trying to get all products");
            }

            // Test the error handling
            // ret = StatusCode(StatusCodes.Status500InternalServerError, "This is an error.");

            return ret;
        }

        [HttpPost("Search")]
        public IActionResult Search([FromBody] ProductSearch search)
        {
            IActionResult ret = null;
            List<Product> list = new List<Product>();

            // SimulateLongRunning();

            try
            {
                if (this.context.Products.Count() > 0)
                {
                    list = this.context.Products
                      .Where(p => p.ProductName.StartsWith(search.ProductName))
                      .OrderBy(p => p.ProductName).ToList();
                    ret = StatusCode(StatusCodes.Status200OK, list);
                    if (list.Count() == 0)
                    {
                        ret = StatusCode(StatusCodes.Status404NotFound, "No Products exist that match the search criteria.");
                    }
                }
                else
                {
                    ret = StatusCode(StatusCodes.Status404NotFound,"No Products exist in the system.");
                }
            }
            catch (Exception ex)
            {
                ret = HandleException(ex, "Exception trying to get all products");
            }

            // Test the error handling
            // ret = StatusCode(StatusCodes.Status500InternalServerError, "This is an error.");

            return ret;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            IActionResult ret = null;
            Product entity = null;

            // SimulateLongRunning();

            try
            {
                entity = this.context.Products.Find(id);

                if (entity != null)
                {
                    ret = StatusCode(StatusCodes.Status200OK, entity);
                }
                else
                {
                    ret = StatusCode(StatusCodes.Status404NotFound, "Can't find Product: " + id.ToString() + ".");
                }
            }
            catch (Exception ex)
            {
                ret = HandleException(ex, "Exception trying to retrieve a single product.");
            }

            // Test the error handling
            // ret = StatusCode(StatusCodes.Status500InternalServerError, "Error Trying to Retrieve a Product.");

            return ret;
        }

        [HttpPost()]
        public IActionResult Post([FromBody] Product entity)
        {
            IActionResult ret = null;

            SimulateLongRunning();

            try
            {
                if (entity != null)
                {
                    this.context.Products.Add(entity);
                    this.context.SaveChanges();

                    ret = StatusCode(StatusCodes.Status201Created, entity);
                }
                else
                {
                    ret = StatusCode(StatusCodes.Status400BadRequest,"Invalid Product object passed to POST method.");
                }
            }
            catch (Exception ex)
            {
                ret = HandleException(ex, "Exception trying to insert a new Product.");
            }

            return ret;
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product entity)
        {
            IActionResult ret = null;

            try
            {
                if (entity != null)
                {
                    this.context.Update(entity);
                    this.context.SaveChanges();
                    ret = StatusCode(StatusCodes.Status200OK, entity);
                }
                else
                {
                    ret = StatusCode(StatusCodes.Status400BadRequest, "Invalid Product object passed to PUT method.");
                }
            }
            catch (Exception ex)
            {
                ret = HandleException(ex,"Exception trying to update Product ID: " + entity.ProductId.ToString() + ".");
            }

            return ret;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            IActionResult ret = null;
            Product entity = null;

            SimulateLongRunning();

            try
            {
                entity = this.context.Products.Find(id);
                if (entity != null)
                {
                    this.context.Products.Remove(entity);
                    this.context.SaveChanges();
                    ret = StatusCode(StatusCodes.Status200OK, true);
                }
                else
                {
                    ret = StatusCode(StatusCodes.Status404NotFound, "Can't find Product ID: " + id.ToString() +" to delete.");
                }
            }
            catch (Exception ex)
            {
                ret = HandleException(ex, "Exception trying to delete product: " + id.ToString());
            }

            return ret;
        }
    }
}
