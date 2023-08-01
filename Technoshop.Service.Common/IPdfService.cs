using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Model;

namespace Technoshop.Service.Common
{
    public interface IPdfService
    {
        PdfDocument Create(Order order);
    }
}
