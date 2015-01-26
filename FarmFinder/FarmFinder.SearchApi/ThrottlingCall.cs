using System;
using System.Threading;
using System.Threading.Tasks;

namespace CH.Tutteli.FarmFinder.SearchApi
{
    public class ThrottlingCall
    {
        private readonly TimeSpan _minTimeBetweenCalls;

        public DateTime LastTimeExecuteWasCalled { get; set; }

        public ThrottlingCall(TimeSpan minTimeBetweenCalls)
        {
            _minTimeBetweenCalls = minTimeBetweenCalls;
        }

        public async Task Execute(Func<Task> func)
        {
            if (LastTimeExecuteWasCalled >= DateTime.Now.Add(-_minTimeBetweenCalls))
            {
                //check if a call is already pending
                //if not, then we set up a delayed call
                if (LastTimeExecuteWasCalled < DateTime.Now)
                {
                    LastTimeExecuteWasCalled = DateTime.Now.Add(_minTimeBetweenCalls);
                    Task.Run(async () =>
                    {
                        Thread.Sleep(_minTimeBetweenCalls);
                        await func();
                    });
                }
            }
            else
            {
                LastTimeExecuteWasCalled = DateTime.Now;
                await func();
            }
        }
    }
}