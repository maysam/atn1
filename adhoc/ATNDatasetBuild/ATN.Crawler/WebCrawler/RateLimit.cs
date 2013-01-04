using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace ATN.Crawler.WebCrawler
{
    /// <summary>
    /// A service which will automatically sleep the calling thread if the number of requests matches or exceeds a set threshold in a static time interval
    /// </summary>
    public class RateLimit
    {
        private TimeSpan RequestInterval = new TimeSpan(0, 0, 60);
        private const int RequestsPerInterval = 150;
        private Stopwatch _watch;
        private uint _requests;

        /// <summary>
        /// The number of requests made in the current time interval
        /// </summary>
        public uint CurrentRequests
        {
            get
            {
                return _requests;
            }
        }

        /// <summary>
        /// The amount of time remaining in the given time interval
        /// </summary>
        public TimeSpan RemainingRequestInterval
        {
            get
            {
                if (RequestInterval >= _watch.Elapsed)
                {
                    return RequestInterval - _watch.Elapsed;
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
                return _watch.Elapsed;
            }
        }
        public RateLimit()
        {
            _watch = new Stopwatch();
            _requests = 0;
        }

        /// <summary>
        /// Add a request to the limiter; this method will block the thread if the request count matches or exceeds the threshold
        /// </summary>
        public void AddRequest()
        {
            if (!_watch.IsRunning)
            {
                _watch.Start();
            }
            _requests++;
            WaitIfLimited();
        }
        
        /// <summary>
        /// Block the calling thread until the time interval for requests has expired
        /// </summary>
        public void WaitIfLimited()
        {
            if (_requests >= RequestsPerInterval && _watch.Elapsed <= RequestInterval)
            {
                TimeSpan WaitSpan = RequestInterval - _watch.Elapsed;
                Trace.WriteLine(string.Format("Hit rate limit wall, waiting {0} before continuing.", WaitSpan), "Informational");
                Thread.Sleep(WaitSpan);
                _watch.Restart();
                _requests = 0;
            }
            else if (_watch.Elapsed > RequestInterval)
            {
                Trace.WriteLine("Restting rate limit.", "Informational");
                _requests = 0;
                _watch.Restart();
            }
        }
    }
}
