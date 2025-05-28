using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Streamling.Utils.EmailHelper;

namespace Streamling.Controllers.Email
{
    public class EmailTestController : BaseApiController
    {
        [HttpPost]
        public async Task SendEmail()
        {
            string toEmail = "dxyrun2023@gmail.com";
            string subject = "Test Email";
            string body = "Body of the email";

            await EmailHelper.SendEmailAsync(toEmail, subject, body);
        }
    }
}