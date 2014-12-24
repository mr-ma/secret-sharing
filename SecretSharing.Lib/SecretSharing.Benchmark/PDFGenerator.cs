using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using System.IO;

namespace SecretSharing.Benchmark
{
   public class PDFGenerator
    {

       public void GenBenchmarkDoc(string filePath, IEnumerable<SecretSharingBenchmarkReport> reports=null)
       {
           if (reports == null || reports.Count() == 0) return;
           Document doc = new Document(PageSize.A4);
           PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
           doc.Open();
           
           PdfPTable table1 = new PdfPTable(12);
           table1.WidthPercentage = 90;


           
           bool firsttime=true;
           ///rest of the header k values
           for (int n = 0; n < 11; n ++)
           {
               PdfPCell[] cells = new PdfPCell[12];
               PdfPCell ncell;
               if (firsttime)
               {
                   ////add header of key size to very left
                   ncell = new PdfPCell();
                   ncell.AddElement(new Paragraph(reports == null ? "80 bits" : reports.First().keyLength + " bits"));
                   firsttime = false;
               }
               else
               {
                   ncell = new PdfPCell();
                   ncell.AddElement(new Paragraph(n == 1 ? "n=5" : (n*5).ToString()));
               }
               cells[0] = ncell;

               for (int k = 0; k < 11; k++)
               {
                   PdfPCell kcell = new PdfPCell();
                   if (n==0)
                   {
                       kcell.AddElement(new Paragraph(k == 0 ? "k=1" : (k * 5).ToString()));
                   } 
                   else if (reports !=null && k <= n )
                   {
                       //add values here

                       var val= reports.Where(po => po.n == n * 5 && po.k == (k==0?1:k*5));
                       if (val.Count()>0)
                       {
                           var re = val.First();
                           kcell.AddElement(new Paragraph(re.avg.TotalMilliseconds.ToString("F0")));
                       }
                   }
                   cells[k + 1] = kcell;
               }
               PdfPRow row = new PdfPRow(cells);
               table1.Rows.Add(row);
           }
           doc.Add(table1);
           doc.Close();

       }

       public void GenHello()
       {
           Document doc = new Document(PageSize.A4);
           PdfWriter.GetInstance(doc, new FileStream("hello.pdf", FileMode.Create));
           doc.Open();
           PdfPTable table1 = new PdfPTable(2);
           table1.WidthPercentage = 90;

           PdfPCell cell11 = new PdfPCell();

           cell11.AddElement(new Paragraph("Receipt ID : "));

           cell11.AddElement(new Paragraph("Date : "));

           cell11.AddElement(new Paragraph("Photo Status : "));

           cell11.VerticalAlignment = Element.ALIGN_LEFT;

           PdfPCell cell12 = new PdfPCell();

           cell12.AddElement(new Paragraph("Transaction ID : "));

           cell12.AddElement(new Paragraph("Expected Date Of Delivery : "));

           cell12.VerticalAlignment = Element.ALIGN_RIGHT;



           table1.AddCell(cell11);

           table1.AddCell(cell12);



           PdfPTable table2 = new PdfPTable(3);



           //One row added

           PdfPCell cell21 = new PdfPCell();

           cell21.AddElement(new Paragraph("Photo Type"));

           PdfPCell cell22 = new PdfPCell();

           cell22.AddElement(new Paragraph("No. of Copies"));

           PdfPCell cell23 = new PdfPCell();

           cell23.AddElement(new Paragraph("Amount"));



           table2.AddCell(cell21);

           table2.AddCell(cell22);

           table2.AddCell(cell23);



           //New Row Added

           PdfPCell cell31 = new PdfPCell();

           cell31.AddElement(new Paragraph("type"));

           cell31.FixedHeight = 300.0f;

           PdfPCell cell32 = new PdfPCell();

           cell32.AddElement(new Paragraph("noofcopy"));

           cell32.FixedHeight = 300.0f;

           PdfPCell cell33 = new PdfPCell();

           cell33.AddElement(new Paragraph("20.00 * "));// + noOfCopy + " = " + (20 * Convert.ToInt32(noOfCopy)) + ".00"));

           cell33.FixedHeight = 300.0f;



           table2.AddCell(cell31);

           table2.AddCell(cell32);

           table2.AddCell(cell33);



           PdfPCell cell2A = new PdfPCell(table2);

           cell2A.Colspan = 2;

           table1.AddCell(cell2A);

           PdfPCell cell41 = new PdfPCell();

           cell41.AddElement(new Paragraph("Name : "));

           cell41.AddElement(new Paragraph("Advance : "));

           cell11.VerticalAlignment = Element.ALIGN_LEFT;

           PdfPCell cell42 = new PdfPCell();

           cell42.AddElement(new Paragraph("Customer ID : "));

           cell42.AddElement(new Paragraph("Balance : "));

           cell42.VerticalAlignment = Element.ALIGN_RIGHT;



           table1.AddCell(cell41);

           table1.AddCell(cell42);



           doc.Add(table1);

           doc.Close();


       }
       public PDFGenerator()
       {
          
       }
    }
}
