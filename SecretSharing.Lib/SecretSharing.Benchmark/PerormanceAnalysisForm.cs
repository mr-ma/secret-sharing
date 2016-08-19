using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecretSharing.Benchmark
{
    public partial class PerormanceAnalysisForm : Form
    {

        private void InitPlotView(){
                 this.plot1 = new OxyPlot.WindowsForms.PlotView();
            this.SuspendLayout();
            this.plot1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plot1.Location = new System.Drawing.Point(0, 0);
            this.plot1.Name = "plot1";
            this.plot1.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plot1.Size = new System.Drawing.Size(484, 312);
            this.plot1.TabIndex = 0;
            this.plot1.Text = "plot1";
            this.plot1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plot1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plot1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;



            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 312);
            this.Controls.Add(this.plot1);
            this.Name = "Form1";
            this.Text = "Example 1 (WindowsForms)";
            this.ResumeLayout(false);
        }
        OxyPlot.WindowsForms.PlotView plot1;

       
        public PerormanceAnalysisForm()
        {
            
            InitializeComponent();
            InitPlotView();

            var myModel = new PlotModel { Title = "Example 1" };
            var lineSeries = new LineSeries("title");
            lineSeries.Points.Add(new DataPoint(-1, 5));
            lineSeries.Points.Add(new DataPoint(2, 6));
            lineSeries.Points.Add(new DataPoint(3, 7));
            lineSeries.Points.Add(new DataPoint(4, 8));

            myModel.Series.Add(lineSeries);
            myModel.Series.Add(new FunctionSeries(Math.Log, 0, 10, 0.1, "log(x)"));
            this.plot1.Model = myModel;
        }



        public void ExportPerformanceReportsAsPNG_PDF(List<IEnumerable<SecretSharingBenchmarkReport>> results, List<string> titles,
            string PlotTitle, bool drawMinusPrimeTime, bool drawElapsedTime, Func<SecretSharingBenchmarkReport,long> selector, string selectorTitle,
            Func<IEnumerable<long>, double> functionToDraw, string functionTitle, string ExportPNGFileName )
        {

            InitializeComponent();
            InitPlotView();

            var myModel = new PlotModel { Title = string.Format("{0}", PlotTitle) };
            myModel.LegendPosition = LegendPosition.TopLeft;
            int i = 0;
            
            if (results ==null || results.Count == 0) return;
            double minMs = double.MaxValue;
            double maxMs = double.MinValue;

            List<SecretSharingBenchmarkReport> temp = new List<SecretSharingBenchmarkReport>();
            var a = results.Select(po => po.ToList()).Aggregate((current, next) =>
            {
                temp.AddRange(current);
                temp.AddRange(next); 
                return temp; 
            });
            long minN;
            long maxN;
            if (temp.Count > 0)
            {
                minN = temp.Min(selector);
                maxN = temp.Max(selector);
            }
            else
            {
                minN = results[0].Min(selector);
                maxN = results[0].Max(selector);
            }

            foreach (var reports in results)
            {
                double sumPrimeGenTime, rmsPrimeTime=0.0;
                

                LineSeries lineNoPrimeSeries = null;
                var lineSeries = new LineSeries(titles[i]);
                
                lineSeries.MarkerType = MarkerType.Circle;
                lineSeries.Smooth = true;
#if calcPrimeTime
                if (drawMinusPrimeTime)
                {
                    sumPrimeGenTime = reports.Select(po => Math.Pow(po.primeGenerationTime, 2.0)).Aggregate((current, next) => current + next);
                    rmsPrimeTime = Math.Sqrt(1.0 / (double)reports.Count() * (sumPrimeGenTime));
                    lineNoPrimeSeries = new LineSeries(titles[i] + "primeGen:" + rmsPrimeTime.ToString());
                    
                }
#endif
              
                
                foreach (var report in reports)
                {
                    // if we have all elapsd ticks we can view the graph as the f(x) that we wish otherwise only totalElapsed is available
                    var ElapsdToDraw =report.ElapsedTicks ==null? report.TotalElapsedMilliseconds: functionToDraw(report.ElapsedTicks);
 
                    if (drawElapsedTime)
                    {
                        maxMs = Math.Max(maxMs, ElapsdToDraw);
                        minMs = Math.Min(minMs, ElapsdToDraw);
                        lineSeries.Points.Add(new DataPoint(selector(report), ElapsdToDraw));
                    }
#if calcPrimeTime
                    if (drawMinusPrimeTime)
                    {
                        //if (report.TotalElapsedMilliseconds - rmsPrimeTime <= 0) throw new Exception("rms is bigger than total elapsed time");
                        if (report.TotalElapsedMilliseconds - rmsPrimeTime/*report.primeGenerationTime*/ < minMs) { minMs = report.TotalElapsedMilliseconds - rmsPrimeTime /*report.primeGenerationTime*/ ;}
                        if (report.TotalElapsedMilliseconds - rmsPrimeTime /*report.primeGenerationTime*/ > maxMs) { maxMs = report.TotalElapsedMilliseconds - rmsPrimeTime /* report.primeGenerationTime*/; }
                        lineNoPrimeSeries.Points.Add(new DataPoint(report.n, report.TotalElapsedMilliseconds - rmsPrimeTime /*report.primeGenerationTime*/));
                    }
#endif
                }
                if (lineSeries.Points.Count > 0)
                {
                    myModel.Series.Add(lineSeries);
                }
                if (lineNoPrimeSeries != null && lineNoPrimeSeries.Points.Count > 0)
                {
                    myModel.Series.Add(lineNoPrimeSeries);
                }
                i++;
            }
            // myModel.Series.Add(new FunctionSeries(nloglogn, 0, 10, 0.1, "N log2(N)"));
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis(OxyPlot.Axes.AxisPosition.Bottom, minN, maxN, selectorTitle));
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis(OxyPlot.Axes.AxisPosition.Left, minMs - 3 < 0 ? 0 : minMs - 3, maxMs, functionTitle));
            this.plot1.Model = myModel;

            //if (ExportPdfFileName != null)
            //{
            using (var stream = File.Create(ExportPNGFileName+".pdf"))
            {
                PdfExporter.Export(myModel, stream, 600, 400);
            }
            OxyPlot.WindowsForms.PngExporter.Export(myModel, ExportPNGFileName + ".png", 600, 400, Brushes.White);
            //}
        }
      
        double nloglogn(double n)
        {
            return n*(Math.Log(n) * Math.Log(n));
        }
        public double StandardDeviation(List<long> values)
        {
            var iterate = values.Count;
            var averageE = values.Average();
            var standardDivSum = values.Select(x => Math.Pow(x - averageE, 2)).Aggregate((current, next) => current + next);
            var standardD = Math.Sqrt((1.0 / ((double)iterate)) * standardDivSum) / TimeSpan.TicksPerMillisecond;
            return standardD;
        }

        public double Average(List<long> values)
        {
            return values.Average();
        }

        internal void ExportPerformanceReportsAsPNG_PDF(List<IEnumerable<BenalohLeichterBenchmarkReport>> results, List<string> titles,
            string PlotTitle, bool drawMinusPrimeTime, bool drawElapsedTime, Func<BenalohLeichterBenchmarkReport,long> selector, string selectorTitle,
            Func<IEnumerable<long>, double> functionToDraw, string functionTitle, string ExportPNGFileName)
        
        {
            InitializeComponent();
            InitPlotView();

            var myModel = new PlotModel { Title = string.Format("{0}", PlotTitle) };
            myModel.LegendPosition = LegendPosition.TopLeft;
            int i = 0;

            if (results == null || results.Count == 0) return;
            double minMs = double.MaxValue;
            double maxMs = double.MinValue;

            List<BenalohLeichterBenchmarkReport> temp = new List<BenalohLeichterBenchmarkReport>();
            var a = results.Select(po => po.ToList()).Aggregate((current, next) =>
            {
                temp.AddRange(current);
                temp.AddRange(next);
                return temp;
            });
            long minN;
            long maxN;
            if (temp.Count > 0)
            {
                minN = temp.Min(selector);
                maxN = temp.Max(selector);
            }
            else
            {
                minN = results[0].Min(selector);
                maxN = results[0].Max(selector);
            }

            foreach (var reports in results)
            {
                double sumPrimeGenTime, rmsPrimeTime = 0.0;


                LineSeries lineNoPrimeSeries = null;
                var lineSeries = new LineSeries(titles[i]);

                lineSeries.MarkerType = MarkerType.Circle;
                lineSeries.Smooth = true;



                foreach (var report in reports)
                {
                    // if we have all elapsd ticks we can view the graph as the f(x) that we wish otherwise only totalElapsed is available
                    var ElapsdToDraw = functionToDraw(report.ElapsedTicks);

                    if (drawElapsedTime)
                    {
                        maxMs = Math.Max(maxMs, ElapsdToDraw);
                        minMs = Math.Min(minMs, ElapsdToDraw);
                        lineSeries.Points.Add(new DataPoint(selector(report), ElapsdToDraw));
                    }

                }
                if (lineSeries.Points.Count > 0)
                {
                    myModel.Series.Add(lineSeries);
                }
                if (lineNoPrimeSeries != null && lineNoPrimeSeries.Points.Count > 0)
                {
                    myModel.Series.Add(lineNoPrimeSeries);
                }
                i++;
            }
            // myModel.Series.Add(new FunctionSeries(nloglogn, 0, 10, 0.1, "N log2(N)"));
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis(OxyPlot.Axes.AxisPosition.Bottom, minN, maxN, selectorTitle));
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis(OxyPlot.Axes.AxisPosition.Left, minMs - 3 < 0 ? 0 : minMs - 3, maxMs, functionTitle));
            this.plot1.Model = myModel;

            //if (ExportPdfFileName != null)
            //{
            using (var stream = File.Create(ExportPNGFileName + ".pdf"))
            {
                PdfExporter.Export(myModel, stream, 600, 400);
            }
            OxyPlot.WindowsForms.PngExporter.Export(myModel, ExportPNGFileName + ".png", 600, 400, Brushes.White);
            //}
        }
    }
}
