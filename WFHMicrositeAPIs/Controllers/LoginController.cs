using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WFHMicrositeAPIs.Models;

namespace MicrositeAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly WFHMicrositeContext _context;

        public LoginController(WFHMicrositeContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<LoginData>> LoginUser(LoginData data)
        {
            var product = await _context.Products.Where(x => x.ProductId == data.ProductId).FirstOrDefaultAsync();
            var user = await _context.Users.Where(x => x.ProductId == data.ProductId && x.EmailAddress == data.EmailAddress && x.Pin == data.PIN).FirstOrDefaultAsync();
            if (user == null || product.Completed)
            {
                return NotFound();
            }
            data.UserId = user.UserId;

            return data;
        }
    }
}