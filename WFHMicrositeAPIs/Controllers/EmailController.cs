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
using Microsoft.Extensions.Configuration;
using WFHMicrositeAPIs.Models;
using IronPdf;
using Microsoft.CodeAnalysis;

namespace WFHMicrositeAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly WFHMicrositeContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public EmailController(WFHMicrositeContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _configuration = configuration;
        }

        [HttpGet("{id}/{emailid}")]
        public async Task<ActionResult> EmailStatus(int id, int emailid)
        {
            User user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            MailMessage emessage = null;
            switch (emailid)
            {
                case 2:
                case 4:
                    emessage = await StatusEmail(user, 2);
                    break;
                case 5:
                    emessage = await OrderEmail(user);
                    break;
            }
            if (emessage != null)
            {
                using (SmtpClient SmtpMail = new SmtpClient("allfs90.allseating.com", 25))
                {
                    SmtpMail.UseDefaultCredentials = true;
                    SmtpMail.Send(emessage);
                    emessage.Dispose();
                    if (emailid == 5)
                    {
                        user.Submitted = DateTime.Now;
                        _context.Entry(user).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                if (emailid == 4)
                {
                    emessage = await OrderEmail(user);
                    if (emessage != null)
                    {
                        using (SmtpClient SmtpMail = new SmtpClient("allfs90.allseating.com", 25))
                        {
                            SmtpMail.UseDefaultCredentials = true;
                            SmtpMail.Send(emessage);
                            emessage.Dispose();
                        }
                    }
                }
            }

            return NoContent();
        }

        private async Task<MailMessage> StatusEmail(User user, int emailid)
        {
            var userSelections = await _context.UserSelections.Where(x => x.UserId == user.UserId).ToListAsync();
            int option1 = 0;
            int option2 = 0;
            int option3 = 0;
            int option4 = 0;
            int option5 = 0;
            foreach (var item in userSelections)
            {
                switch (item.Type)
                {
                    case "Fabric":
                        option1 = item.ProductOptionId;
                        break;
                    case "Mesh":
                        option2 = item.ProductOptionId;
                        break;
                    case "Frame":
                        option3 = item.ProductOptionId;
                        break;
                    case "Arms":
                        option4 = item.ProductOptionId;
                        break;
                    case "Castors":
                        option5 = item.ProductOptionId;
                        break;
                }
            }
            user.SessionId = Guid.NewGuid().ToString().Replace("-", "");
            string url = _configuration.GetValue<string>("AppSettings:CompleteUrl") + user.SessionId;
            _context.Update(user);
            await _context.SaveChangesAsync();
            string body = "Your address and selections have been saved.<br/><br/><a href='" + url + ">Click here</a> to review your selections.";
            string file = _env.WebRootPath + "\\emails\\email" + emailid.ToString() + "_" + user.Language + ".txt";
            StreamReader sr = new StreamReader(file, System.Text.Encoding.UTF8);
            if (sr != null)
            {
                string[] parameters = new string[] { user.UserId.ToString().PadLeft(8, '0'), url };
                body = string.Format(sr.ReadToEnd(), parameters);
                sr.Close();
                sr.Dispose();
            }
            string subject = "";
            switch (emailid)
            {
                case 2:
                    subject = user.Language == "English" ? "Thank you for your order!" : "Nous vous remercions de votre commande!";
                    break;

            }
            var emessage = new MailMessage("postmaster@allseating.com", user.EmailAddress, subject, body);
            emessage.IsBodyHtml = true;
            emessage.BodyEncoding = System.Text.Encoding.UTF8;
            emessage.Bcc.Add("admin@allseating.com");
            emessage.Bcc.Add("wfh@allseating.com");

            return emessage;
        }

        private async Task<MailMessage> OrderEmail(User user)
        {
            string emailAddress = _configuration.GetValue<string>("AppSettings:AllInEmail");
            Product product = await _context.Products.Where(x => x.ProductId == user.ProductId).FirstOrDefaultAsync();
            List<UserSelection> userSelections = await _context.UserSelections.Where(x => x.UserId == user.UserId).ToListAsync();
            string poNumber = product.Ponumber + "-" + user.UserId.ToString();
            string altPo = await _context.AlternatePonumbers.Where(x => x.UserId == user.UserId).Select(y => y.AlternatePonumber1).FirstOrDefaultAsync();
            if (!string.IsNullOrWhiteSpace(altPo))
            {
                poNumber = altPo.Trim() + "-" + user.UserId.ToString();
            }
            string subject = "WFH Order " + user.UserId.ToString();
            string body = "The sif file for the order is attached.";

            string fileName = @"c:\temp\" + user.UserId.ToString() + ".sif";
            string sifFile = _env.WebRootPath + @"\sifs\" + product.Config + ".sif";
            if (System.IO.File.Exists(fileName))
            {
                try { System.IO.File.Delete(fileName); }
                catch { }
            }
            var emessage = new MailMessage("postmaster@allseating.com", emailAddress, subject, body);
            emessage.BodyEncoding = System.Text.Encoding.UTF8;
            StreamReader sr = new StreamReader(sifFile);
            if (sr != null)
            {
                string[] parameters = new string[]
                {
                        poNumber,
                        product.DealerCode,
                        "1",
                        "1.00",
                        user.EmailAddress.ToLower()
                };
                var sif = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                sif = string.Format(sif, parameters);
                string[] option;
                ProductOption productOption;
                foreach (var item in userSelections)
                {
                    productOption = await _context.ProductOptions.Where(x => x.ProductOptionId == item.ProductOptionId).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(productOption.StockCode))
                    {
                        option = productOption.StockCode.Split('~');
                        sif += "ON=" + option[0] + "\r\n";
                        sif += "OD=" + option[1] + "\r\n";
                    }
                }
                StreamWriter sw = new StreamWriter(fileName);
                sw.WriteLine(sif);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            if (System.IO.File.Exists(fileName))
            {
                emessage.Attachments.Add(new Attachment(fileName));
            }
            fileName = @"c:\temp\" + user.UserId.ToString() + "_details.pdf";
            string orderFile = _env.WebRootPath + @"\emails\order_details.html";
            if (System.IO.File.Exists(fileName))
            {
                try { System.IO.File.Delete(fileName); }
                catch { }
            }
            sr = new StreamReader(orderFile);
            if (sr != null)
            {
                string options = "";
                ProductOption option;
                foreach (var item in userSelections)
                {
                    option = await _context.ProductOptions.Where(x => x.ProductOptionId == item.ProductOptionId).FirstOrDefaultAsync();
                    if (!string.IsNullOrEmpty(option.StockCode))
                    {
                        options += "<tr><td style=\"width: 30%; font-size:small; font-weight: bold;\"><label>" + option.Type + "</label></td><td style=\"width: 35%; font-size:small;\"><label>" + option.Name + "</label></td><td style=\"width: 35%; font-size:small;\"><label>" + option.StockCode + "</label></td></tr>";
                    }
                }
                string[] parameters = new string[]
                {
                        poNumber,
                        user.UserId.ToString(),
                        "1.00",
                        user.EmailAddress.ToUpper(),
                        user.PhoneNumber,
                        user.Address1.ToUpper(),
                        string.IsNullOrWhiteSpace(user.Address2) ? "" : user.Address2.ToUpper(),
                        user.City.ToUpper(),
                        user.ProvinceState.ToUpper(),
                        user.PostalZip.ToUpper(),
                        user.Country.ToUpper(),
                        string.IsNullOrWhiteSpace(user.SpecialInstructions) ? "" : user.SpecialInstructions.ToUpper(),
                        product.Name,
                        options
                };
                var html = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                var htmlToPdf = new HtmlToPdf();
                try
                {
                    html = string.Format(html, parameters);
                    var pdf = htmlToPdf.RenderHtmlAsPdf(html);
                    pdf.SaveAs(fileName);
                }
                catch { }
            }
            if (System.IO.File.Exists(fileName))
            {
                emessage.Attachments.Add(new Attachment(fileName));
            }
            return emessage;
        }

        private string GetRandomNumber()
        {
            string number = "";
            do
            {
                number = new Random().Next(100000000, 999999999).ToString();
                if (_context.Users.Where(x => x.SessionId == number).FirstOrDefault() != null)
                {
                    number = "";
                }
            } while (number == "");

            return number;
        }
    }
}