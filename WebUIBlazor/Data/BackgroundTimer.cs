
namespace WebUI.Data
{
    public class BackgroundTimer
    {


        public readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(1000));
        public CancellationTokenSource _cts = new();

        public event Action SecondElapsed;

        BackgroundTimer()
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
            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    //Console.WriteLine(DateTime.Now);
                    SecondElapsed?.Invoke();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Timer Cancelled");
            }
        }
    }
}
