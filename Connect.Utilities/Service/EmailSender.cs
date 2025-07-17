using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Connect.Utilities.Service
{
    public  class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //logic to send email
            return Task.CompletedTask;
        }
    }
}
