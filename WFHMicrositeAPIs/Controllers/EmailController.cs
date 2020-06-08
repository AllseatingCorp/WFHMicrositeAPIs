using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WFHMicrositeAPIs.Models;
using Microsoft.CodeAnalysis;

namespace WFHMicrositeAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly WFHMicrositeContext _context;
        private readonly IWebHostEnvironment _env;

        public EmailController(WFHMicrositeContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("{id}/{emailid}")]
        public async Task<ActionResult> EmailStatus(int id, int emailid)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userSelections = await _context.UserSelection.Where(x => x.UserId == id).ToListAsync();
            int option1 = 0;
            int option2 = 0;
            foreach (var item in userSelections)
            {
                if (item.Type == "Fabric")
                {
                    option1 = item.ProductOptionId;
                }
                else
                {
                    option2 = item.ProductOptionId;
                }
            }
            var productImage = await _context.ProductImage.Where(x => x.ProductId == user.ProductId && x.ProductOption1Id == option1 && x.ProductOption2Id == option2).FirstOrDefaultAsync();
            string url = "";
            string body = "Your address and selections have been saved.<br/><br/><a href='" + url + ">Click here</a> to review your selections.";
            string file = _env.WebRootPath + "\\emails\\" + user.Language + "\\email" + emailid.ToString() + ".txt";
            StreamReader sr = new StreamReader(file);
            if (sr != null)
            {
                string[] parameters = new string[] { user.UserId.ToString().PadLeft(8,'0'), url };
                body = string.Format(sr.ReadToEnd(), parameters);
                sr.Close();
                sr.Dispose();
            }
            var emessage = new MailMessage("postmaster@allseating.com", user.EmailAddress, "Thank you for your order!", body);
            if (productImage != null)
            {
                emessage.Attachments.Add(new Attachment(new MemoryStream(productImage.Image), "Your Chair"));
                emessage.Attachments[0].ContentId = "imageRef";
            }
            emessage.IsBodyHtml = true;
            using (SmtpClient SmtpMail = new SmtpClient("allfs90.allseating.com", 25))
            {
                SmtpMail.UseDefaultCredentials = true;
                SmtpMail.Send(emessage);
                emessage.Dispose();
            }

            return NoContent();
        }
    }
}