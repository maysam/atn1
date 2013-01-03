using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Crawler.WebCrawler
{
    public class RateLimit
    {
        private TimeSpan RequestInterval = new TimeSpan(0, 0, 30);
        private const int RequestsPerInterval = 30;
        private Stopwatch _watch;
        private uint _requests;
        public uint CurrentRequests
        {
            get
            {
                return _requests;
            }
        }
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
        public void AddRequest()
        {
            if (!_watch.IsRunning)
            {
                _watch.Start();
            }
            _requests++;
            WaitIfLimited();
        }
        public void WaitIfLimited()
        {
            if (_requests >= RequestsPerInterval && _watch.Elapsed <= RequestInterval)
            {
                TimeSpan WaitSpan = RequestInterval - _watch.Elapsed;
                Console.WriteLine("Hit rate limit wall, waiting {0} before continuing.", WaitSpan);
                Thread.Sleep(WaitSpan);
                _watch.Restart();
                _requests = 0;
            }
            else if (_watch.Elapsed > RequestInterval)
            {
                Console.WriteLine("Restting rate limit.");
                _requests = 0;
                _watch.Restart();
            }
        }
    }
}
