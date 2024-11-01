using System.Text;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Paragraph = iTextSharp.text.Paragraph;

namespace Api.Controllers;

public class TestController : BaseController
{
    [HttpGet("[action]")]
    public IActionResult GeneratePdfController()
    {
        var templatePath = "C:\\Users\\DKDT\\Desktop\\Final_Umz_Project\\Common\\Template\\template.docx";
        var templateContent = System.IO.File.ReadAllText(templatePath, Encoding.UTF8);

        var textContent = new StringBuilder();
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templatePath, false))
        {
            // Extract text from the document
            foreach (var text in wordDoc.MainDocumentPart.Document.Body.Descendants<Text>())
            {
                textContent.Append(text.Text);
            }
        }

        textContent.ToString()
            .Replace("@student_number", "test")
            .Replace("@fullname", "test")
            .Replace("@practice_title", "test")
            .Replace("@practice_answer", "test");


        using var memoryStream = new MemoryStream();
        var document = new Document();
        PdfWriter.GetInstance(document, memoryStream);
        document.Open();

        document.Add(new Paragraph(templateContent));

        document.Close();


        var pdfBytes = memoryStream.ToArray();

        return File(pdfBytes, "application/pdf", "example.pdf");
    }
}