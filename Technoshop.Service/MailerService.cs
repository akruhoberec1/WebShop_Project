using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Service.Common;
using System.Configuration;
using Technoshop.Model;

namespace Technoshop.Service
{
    public class MailerService : IMailerService
    {

        private readonly string _sendGridApiKey;

        public MailerService()
        {
            _sendGridApiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
        }
        



        public async Task SendMail(Order order)
        {
            string orderId = order.Id.ToString();
            string customerName = order.Person.FirstName + ' ' + order.Person.LastName;

            var to = new EmailAddress(order.Person.Email, customerName);
            SendGridMessage message = new SendGridMessage
            {
                From = new EmailAddress()
                {
                    Name = "Jason Bourne",
                    Email = "jason@technoshop.hr"
                },
                Subject = "Technoshop order created!",
                HtmlContent = $"An order with ID {orderId} has been created for customer {customerName}."
            };

            message.AddTo(to);


            var client = new SendGridClient(_sendGridApiKey);

            var response = await client.SendEmailAsync(message);

   
        }

    }
}
