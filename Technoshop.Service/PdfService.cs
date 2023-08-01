using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Model;
using Technoshop.Service.Common;

namespace Technoshop.Service
{
    public class PdfService : IPdfService
    {
        public PdfDocument Create(Order order)
        {

            PdfDocument pdf = new PdfDocument();
            PdfPage page = pdf.AddPage();

            page.Orientation = PdfSharp.PageOrientation.Portrait;
            page.Size = PdfSharp.PageSize.A4;
            string customerName = order.Person.FirstName + " " + order.Person.LastName;
            Guid orderId = order.Id;

            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont fontTitle = new XFont("Arial", 16, XFontStyle.Bold);
            XFont fontBody = new XFont("Arial", 12);

            string personData = $"Customer: {customerName}\nEmail: {order.Person.Email}\nPhone: {order.Person.Phone}";
            gfx.DrawString(personData, fontBody, XBrushes.Black, new XPoint(50, 50));


            string orderIdText = $"Order ID: {orderId}";
            XSize orderIdSize = gfx.MeasureString(orderIdText, fontTitle);
            double orderIdX = (page.Width - orderIdSize.Width) / 2;
            gfx.DrawString(orderIdText, fontTitle, XBrushes.Black, new XPoint(orderIdX, 200));

            int productCount = order.Products.Count;
            double productsStartY = 300;
            double productLineHeight = 20;
            decimal? totalPrice = 0;

            for (int i = 0; i < productCount; i++)
            {
                var product = order.Products[i];
                string productText = $"{product.Name} - Quantity: {product.Quantity} - Price: {Math.Round(product.Price ?? 0,2)}€";
                double productY = productsStartY + i * productLineHeight;

                gfx.DrawString(productText, fontBody, XBrushes.Black, new XPoint(50, productY));

                totalPrice += product.Price * product.Quantity;
            }
            decimal roundedTotalPrice = Math.Round(totalPrice ?? 0,2);
            string totalPriceText = $"Total Price: {roundedTotalPrice}€";
            double totalPriceY = productsStartY + productCount * productLineHeight + 20;
            gfx.DrawString(totalPriceText, fontBody, XBrushes.Black, new XPoint(50, totalPriceY));

            string desktopFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"order{order.Id}_{order.Person.FirstName}_{order.Person.LastName}_{order.TotalPrice}.pdf";
            string filePath = Path.Combine(desktopFolderPath, fileName);

            pdf.Save(filePath);

            gfx.Dispose();
            pdf.Close();

            return pdf;

        }
    }
}
