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
using ATN.Analysis;
using ATN.Crawler;
using ATN.Data;

namespace ATN.Processing
{
    public partial class ProcessingService : ServiceBase
    {
        private Thread ProcessingThread;
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
                using (CrawlRunner cr = new CrawlRunner())
                {
                    try
                    {
                        ExistingCrawlSpecifier[] ChangedCrawls = cr.ProcessCurrentCrawls();
                        if (ChangedCrawls.Length > 0)
                        {
                            AnalysisInterface ai = new AnalysisInterface();
                            for(int i = 0; i < ChangedCrawls.Length; i++)
                            {
                                int RunId = ai.InitiateTheoryAnalysis(ChangedCrawls[i].TheoryId, ChangedCrawls[i].ImpactFactor);
                                if (ChangedCrawls[i].AEF)
                                {
                                    AEF AEF = new AEF();
                                    AEF.ComputeAndStoreAEF(ChangedCrawls[i].TheoryId, RunId);
                                    if (ChangedCrawls[i].TAR)
                                    {
                                        //Compute TAR
                                    }
                                }
                                //Run ML
                                cr.CompleteAnalysisOnCrawl(ChangedCrawls[i]);
                            }
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
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }
        protected override void OnStart(string[] args)
        {
            ProcessingThread = new Thread(new ThreadStart(PersistService));
            ProcessingThread.Start();
        }

        protected override void OnStop()
        {
            ProcessingThread.Abort();
        }
    }
}
