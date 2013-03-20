using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ATN.Crawler;
using ATN.Data;

namespace ATN.Processing
{
    public partial class ProcessingService : ServiceBase
    {
        private Thread ProcessingThread;
        CrawlRunner co;
        public ProcessingService()
        {
            ServiceName = "ATN Processing";
            EventLog.Log = "Application";

            InitializeComponent();
        }
        private void PersistService()
        {
            while (true)
            {
                try
                {
                    ExistingCrawlSpecifier[] ChangedCrawls = co.ProcessCurrentCrawls();
                    if (ChangedCrawls.Length > 0)
                    {
                        //Do analysis on changed crawls
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Exception:", "Error");
                    Trace.WriteLine(e.Message);
                    Trace.WriteLine(e.Source);
                    Trace.WriteLine(e.StackTrace);
                    Trace.WriteLine(e.TargetSite);
                    Trace.WriteLine(e.Data);
                }
                finally
                {
                    //Do cleanup
                    co.Cleanup();
                    co = null;
                    GC.Collect();
                    co = new CrawlRunner();
                }
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }
        protected override void OnStart(string[] args)
        {
            co = new CrawlRunner();
            ProcessingThread = new Thread(new ThreadStart(PersistService));
            ProcessingThread.Start();
        }

        protected override void OnStop()
        {
            ProcessingThread.Abort();
        }
    }
}
