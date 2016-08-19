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

        public struct TempValueHolder
        {
            public int n;
            public int k;
            public double avg;
            public int keysize;
        }
        public static Dictionary<string, double> AggeragatedReconPhase = new Dictionary<string, double>();
        public PdfPTable TableGenerator(int Columns, int Rows, string keysize, IEnumerable<SecretSharingBenchmarkReport> reports, IEnumerable<SecretSharingBenchmarkReport> comparereports = null)
        {
            List<double> improvments = new List<double>();
            PdfPTable table1 = new PdfPTable(Columns);
            table1.WidthPercentage = 100;
            for (int r = 0; r < Rows; r++)
            {
                PdfPCell[] cells = new PdfPCell[Columns];
                for (int c = 0; c < Columns; c++)
                {
                    PdfPCell columncell = new PdfPCell();
                    if (c == 0) //add left top header column
                    {
                        if (r == 0) columncell.AddElement(new Paragraph(keysize));
                        else if (r == 1)
                            columncell.AddElement(new Paragraph("n=5"));
                        else
                            columncell.AddElement(new Paragraph((r * 5).ToString()));
                    }
                    else if (r == 0 && c != 0)
                    {
                        if (c == 1) columncell.AddElement(new Paragraph("k=5"));
                        else
                            columncell.AddElement(new Paragraph((c * 5).ToString()));
                    }
                    else if (reports != null && c <= r)
                    {
                        //Add corresponidng values from report

                        var val = reports.Where(po => po.n == r * 5 && po.k == (c * 5));

                        if (val.Count() > 0)
                        {
                            var re = val.First().ElapsedTicks.Average() / TimeSpan.TicksPerMillisecond;
                            var firstValopr = val.First().Operation;
                            if (firstValopr == SecretSharingBenchmarkReport.OperationType.RandomSecretReconstruction || firstValopr == SecretSharingBenchmarkReport.OperationType.DecryptShares || firstValopr == SecretSharingBenchmarkReport.OperationType.VerifyPooledShares)
                            {
                                var nstr = (r * 5).ToString();
                                var kstr = (c * 5).ToString();
                                var keystr = keysize+ nstr + ";" + kstr;
                                var oldvalue = 0.0d;
                                if (AggeragatedReconPhase.ContainsKey(keystr))
                                {
                                    oldvalue = AggeragatedReconPhase[keystr];
                                }
                                AggeragatedReconPhase[keystr] = oldvalue + re;
                            }
                            columncell.AddElement(new Paragraph((re).ToString("F2")));
                            if (comparereports != null)
                            {
                                var compareval = comparereports.Where(po => po.n == r * 5 && po.k == (c * 5) && po.chunkSize == po.keyLength/8);

                                if (compareval.Count() > 0)
                                {
                                    double comprativeResult = 0.00d;
                                    if (compareval.First().ElapsedTicks == null)
                                    {
                                        comprativeResult = compareval.Single().TotalElapsedMilliseconds;
                                    }
                                    else
                                    {
                                        comprativeResult = compareval.Single().ElapsedTicks.Average() / TimeSpan.TicksPerMillisecond;
                                    }
                                    columncell.AddElement(new Paragraph(comprativeResult.ToString("F2")));
                                    //columncell.AddElement(new Paragraph((compareval.Single().TotalElapsedMilliseconds).ToString()));
                                    var improvement = (compareval.Single().TotalElapsedMilliseconds / re);
                                    columncell.AddElement(new Paragraph((improvement).ToString("F2")));
                                    improvments.Add(improvement);
                                }
                            }
                        }
                        //columncell.AddElement(new Paragraph("hello"));
                    }
                    cells[c] = columncell;
                }
                PdfPRow row = new PdfPRow(cells);
                table1.Rows.Add(row);
            }


            if (improvments.Count > 0)
            {
                PdfPCell[] cells = new PdfPCell[Columns];
                for (int i = 0; i < cells.Length; i++)
                {
                    cells[i] = new PdfPCell();
                }
                cells[0].AddElement(new Paragraph(string.Format("average:{0} std:{1}", improvments.Average(), StandardDeviation(improvments))));
                table1.Rows.Add(new PdfPRow(cells));
            }

            return table1;

        }

        public PdfPTable TableAggregativeGenerator(int Columns, int Rows, string keysize, bool printImproves, IEnumerable<SecretSharingBenchmarkReport> comparereports = null)
        {
            List<double> improvments = new List<double>();
            PdfPTable table1 = new PdfPTable(Columns);
            table1.WidthPercentage = 100;
            for (int r = 0; r < Rows; r++)
            {
                PdfPCell[] cells = new PdfPCell[Columns];
                for (int c = 0; c < Columns; c++)
                {
                    PdfPCell columncell = new PdfPCell();
                    if (c == 0) //add left top header column
                    {
                        if (r == 0) columncell.AddElement(new Paragraph(keysize));
                        else if (r == 1)
                            columncell.AddElement(new Paragraph("n=5"));
                        else
                            columncell.AddElement(new Paragraph((r * 5).ToString()));
                    }
                    else if (r == 0 && c != 0)
                    {
                        if (c == 1) columncell.AddElement(new Paragraph("k=5"));
                        else
                            columncell.AddElement(new Paragraph((c * 5).ToString()));
                    }
                    else if (AggeragatedReconPhase != null && c <= r)
                    {
                        //Add corresponidng values from report
                        var nstr = (r * 5).ToString();
                        var kstr = (c * 5).ToString();
                        var keystr = keysize + nstr + ";" + kstr;
                        var re = AggeragatedReconPhase[keystr];
                        columncell.AddElement(new Paragraph((re).ToString("F2")));
                        if (comparereports != null)
                        {
                            var compareval = comparereports.Where(po => po.n == r * 5 && po.k == (c * 5) && po.chunkSize ==po.keyLength/8);
                            if (compareval.Count() > 0)
                            {
                                double comprativeResult = 0.00d;
                                if (compareval.First().ElapsedTicks == null)
                                {
                                    comprativeResult = compareval.Single().TotalElapsedMilliseconds;
                                }
                                else
                                {
                                    comprativeResult = compareval.Single().ElapsedTicks.Average() / TimeSpan.TicksPerMillisecond;
                                }
                                columncell.AddElement(new Paragraph(comprativeResult.ToString("F2")));
                                var improvement = (compareval.Single().TotalElapsedMilliseconds / re);
                                if (printImproves)
                                {
                                    columncell.AddElement(new Paragraph((improvement).ToString("F2")));
                                    improvments.Add(improvement);
                                }
                            }
                        }
                        //columncell.AddElement(new Paragraph("hello"));
                    }
                    cells[c] = columncell;
                }
                PdfPRow row = new PdfPRow(cells);
                table1.Rows.Add(row);
            }


            if (improvments.Count > 0)
            {
                PdfPCell[] cells = new PdfPCell[Columns];
                for (int i = 0; i < cells.Length; i++)
                {
                    cells[i] = new PdfPCell();
                }
                cells[0].AddElement(new Paragraph(string.Format("average:{0} std:{1}", improvments.Average(), StandardDeviation(improvments))));
                table1.Rows.Add(new PdfPRow(cells));
            }

            return table1;

        }

        public double StandardDeviation(List<double> values)
        {
            var iterate = values.Count;
            var averageE = values.Average();
            var standardDivSum = values.Select(x => Math.Pow(x - averageE, 2)).Aggregate((current, next) => current + next);
            var standardD = Math.Sqrt((1.0 / ((double)iterate)) * standardDivSum);
            return standardD;
        }


        public void GenBenchmarkDoc(string filePath, IEnumerable<SecretSharingBenchmarkReport> reports, IEnumerable<SecretSharingBenchmarkReport> comparereports = null)
        {
            if (reports == null || reports.Count() == 0) return;
            var pgSize = new iTextSharp.text.Rectangle(620, comparereports == null ? 275 : 730);
            var doc = new iTextSharp.text.Document(pgSize, 10, 10, 10, 10);
            //Document doc = new Document(PageSize.LETTER_LANDSCAPE);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();




            var table1 = TableGenerator(11, 11, reports.First().keyLength / 2 + " bit", reports, comparereports);
            doc.Add(table1);
            doc.Close();

        }

        public void GenAggreagativeBenchmarkDoc(string filePath, int keyLength, bool printImproves,IEnumerable<SecretSharingBenchmarkReport> comparereports = null)
        {
            var pgSize = new iTextSharp.text.Rectangle(620, comparereports == null ? 275 : printImproves? 730:425);
            var doc = new iTextSharp.text.Document(pgSize, 10, 10, 10, 10);
            //Document doc = new Document(PageSize.LETTER_LANDSCAPE);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();




            var table1 = TableAggregativeGenerator(11, 11, keyLength + " bit", printImproves,comparereports);
            doc.Add(table1);
            doc.Close();

        }


        public PDFGenerator()
        {

        }
    }
}
