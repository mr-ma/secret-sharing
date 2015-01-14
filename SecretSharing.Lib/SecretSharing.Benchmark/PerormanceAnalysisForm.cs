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



        public PerormanceAnalysisForm(List<IEnumerable<SecretSharingBenchmarkReport>> results, List<string> titles, string PlotTitle,bool drawWithoutPrimeTime,string ExportPdfFileName = null)
        {

            InitializeComponent();
            InitPlotView();

            var myModel = new PlotModel { Title = string.Format("Performance Report {0}", PlotTitle) };
            int i = 0;
            long maxMs = 0;
            if (results.Count == 0) return;
            long minMs = results[0].FirstOrDefault().avgTotalMiliseconds;
            long minN = results[0].FirstOrDefault().n;
            long maxN = 0;
            foreach (var reports in results)
            {
                LineSeries lineNoPrimeSeries = null;
                var lineSeries = new LineSeries(titles[i]);
                lineSeries.MarkerType = MarkerType.Circle;
                lineSeries.Smooth = true;
                if(drawWithoutPrimeTime)
                {
                    lineNoPrimeSeries = new LineSeries(titles[i] + " no prime");
                }
                foreach (var report in reports)
                {
                    if (report.avgTotalMiliseconds < minMs) minMs = report.avgTotalMiliseconds;
                    if (report.n < minN) minN = report.n;
                    if (report.n > maxN) maxN = report.n;
                    if (report.avgTotalMiliseconds > maxMs) { maxMs = report.avgTotalMiliseconds; }
                    lineSeries.Points.Add(new DataPoint(report.n, report.avgTotalMiliseconds));
                    if (drawWithoutPrimeTime)
                    {
                        if (report.avgTotalMiliseconds - report.primeGenerationTime < minMs) { minMs =report.avgTotalMiliseconds - report.primeGenerationTime; }
                        if (report.avgTotalMiliseconds - report.primeGenerationTime > maxMs) { maxMs =report.avgTotalMiliseconds - report.primeGenerationTime; }
                        lineNoPrimeSeries.Points.Add(new DataPoint(report.n, report.avgTotalMiliseconds - report.primeGenerationTime ));
                    }
                }
                if (lineSeries.Points.Count > 0)
                {
                    myModel.Series.Add(lineSeries);
                }
                if (lineNoPrimeSeries != null &&lineNoPrimeSeries.Points.Count > 0)
                {
                    myModel.Series.Add(lineNoPrimeSeries);
                }
                i++;
            }
           // myModel.Series.Add(new FunctionSeries(nloglogn, 0, 10, 0.1, "N log2(N)"));
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis(OxyPlot.Axes.AxisPosition.Bottom, minN,maxN, "N"));
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis(OxyPlot.Axes.AxisPosition.Left, minMs -3 <0? 0:minMs -3 ,maxMs, "ms"));
            this.plot1.Model = myModel;

            if (ExportPdfFileName != null)
            {
                using (var stream = File.Create(ExportPdfFileName))
                {
                    PdfExporter.Export(myModel, stream, 600, 400);
                    OxyPlot.WindowsForms.PngExporter.Export(myModel, ExportPdfFileName+".png", 600, 400, Brushes.White);
                }
            }
        }

        double nloglogn(double n)
        {
            return n*(Math.Log(n) * Math.Log(n));
        }
    }
}
