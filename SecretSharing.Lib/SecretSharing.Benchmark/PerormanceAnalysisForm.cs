using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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



        public PerormanceAnalysisForm(List<IEnumerable<SecretSharingBenchmarkReport>> results, List<string> titles)
        {

            InitializeComponent();
            InitPlotView();

            var myModel = new PlotModel { Title = "Performance Report" };
            int i = 0;
            foreach (var reports in results)
            {
                var lineSeries = new LineSeries(titles[i]);

                foreach (var report in reports)
                {
                    lineSeries.Points.Add(new DataPoint(report.n, report.avg.TotalSeconds*100));
                }
                myModel.Series.Add(lineSeries);
                i++;
            }

           
            myModel.Series.Add(new FunctionSeries(nloglogn, 0, 10, 0.1, "N log2(N)"));
            this.plot1.Model = myModel;
        }

        double nloglogn(double n)
        {
            return n*(Math.Log(n) * Math.Log(n));
        }
    }
}
