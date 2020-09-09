using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TPMApi.Models;

namespace TPMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WooController : ControllerBase
    {
        public IActionResult GetProducts(AuthorizationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            throw new NotImplementedException();
        }
    }
}