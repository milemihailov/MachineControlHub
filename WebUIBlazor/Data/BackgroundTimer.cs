﻿namespace WebUI.Data
{
    public class BackgroundTimer
    {
        public readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(10));

        public CancellationTokenSource _cts = new();

        public event Action TenMilisecondsElapsed;
        public event Action SecondElapsed;
        public event Action FiveSecondsElapsed;

        public BackgroundTimer(string portName = null)
        {
            StartTimer();
        }

        private static BackgroundTimer _instance;
        public static BackgroundTimer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BackgroundTimer();
                }
                return _instance;
            }
        }

        public void StartTimer()
        {
            _ = DoWorkAsync();
        }

        public void StopTimer()
        {
            _cts.Cancel();
        }


        public async Task DoWorkAsync()
        {
            int i = 0;

            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    TenMilisecondsElapsed?.Invoke();

                    i++;
                    if (i % 100 == 0)
                    {
                        SecondElapsed?.Invoke();
                    }

                    if (i % 500 == 0)
                    {
                        FiveSecondsElapsed?.Invoke();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Timer Cancelled");
            }
        }
    }

}
