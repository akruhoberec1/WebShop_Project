using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Model;

namespace Technoshop.Service.Common
{
    public interface IMailerService
    {

        Task SendMail(Order order);

    }
}
