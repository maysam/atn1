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

namespace ATN.Processing
{
    public partial class CrawlerService : ServiceBase
    {
        private Thread CrawlThread;
        CrawlRunner co;
        public CrawlerService()
        {
            ServiceName = "ATN Crawler";
            EventLog.Log = "Application";

            InitializeComponent();
        }
        private void PersistService()
        {
            while (true)
            {
                co.ProcessStaleCrawls();
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }
        protected override void OnStart(string[] args)
        {
            co = new CrawlRunner();
            CrawlThread = new Thread(new ThreadStart(PersistService));
            CrawlThread.Start();
        }

        protected override void OnStop()
        {
            CrawlThread.Abort();
        }
    }
}
