using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace ATN.Crawler.WebCrawler
{

    //To-do: Make this static abstract, and allow specification of different RateLimit classes for different crawlers

    /// <summary>
    /// A service which will automatically sleep the calling thread if the number of requests matches or exceeds a set threshold in a static time interval
    /// </summary>
    public class RateLimit
    {
        private TimeSpan RequestInterval = new TimeSpan(0, 0, 60);

        private const int RequestsPerInterval = 120;
        private static Stopwatch Watch;
        private static uint? Requests;

        /// <summary>
        /// The number of requests made in the current time interval
        /// </summary>
        public uint CurrentRequests
        {
            get
            {
                return RateLimit.Requests.Value;
            }
        }

        /// <summary>
        /// The amount of time remaining in the given time interval
        /// </summary>
        public TimeSpan RemainingRequestInterval
        {
            get
            {
                if (RequestInterval >= RateLimit.Watch.Elapsed)
                {
                    return RequestInterval - RateLimit.Watch.Elapsed;
                }
                else
                {
                    return new TimeSpan(0);
                }
            }
        }

        /// <summary>
        /// The length of the current time interval
        /// </summary>
        public TimeSpan CurrentRequestInterval
        {
            get
            {
                return RateLimit.Watch.Elapsed;
            }
        }
        public RateLimit()
        {
            if (RateLimit.Watch == null)
            {
                RateLimit.Watch = new Stopwatch();
            }
            if (!RateLimit.Requests.HasValue)
            {
                RateLimit.Requests = 0;
            }
        }

        /// <summary>
        /// Add a request to the limiter; this method will block the thread if the request count matches or exceeds the threshold
        /// </summary>
        public void AddRequest()
        {
            if (!RateLimit.Watch.IsRunning)
            {
                RateLimit.Watch.Start();
            }
            RateLimit.Requests++;
            WaitIfLimited();
        }
        
        /// <summary>
        /// Block the calling thread until the time interval for requests has expired
        /// </summary>
        public void WaitIfLimited()
        {
            if (RateLimit.Requests >= RequestsPerInterval && RateLimit.Watch.Elapsed <= RequestInterval)
            {
                TimeSpan WaitSpan = RequestInterval - RateLimit.Watch.Elapsed;
                Trace.WriteLine(string.Format("Hit rate limit wall, waiting {0} before continuing.", WaitSpan), "Informational");
                Thread.Sleep(WaitSpan);
                RateLimit.Watch.Restart();
                RateLimit.Requests = 0;
            }
            else if (RateLimit.Watch.Elapsed > RequestInterval)
            {
                Trace.WriteLine("Restting rate limit.", "Informational");
                RateLimit.Requests = 0;
                RateLimit.Watch.Restart();
            }
        }
    }
}
