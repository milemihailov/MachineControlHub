﻿
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace WebUI.Data
{
    public class BackgroundTimer
    {
        public readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(10));

        public CancellationTokenSource _cts = new();
        public Stopwatch stopwatch = new();

        public event Action TenMilisecondsElapsed;
        public event Action SecondElapsed;

        [Parameter]
        public bool IsBusy { get; set; }

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
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Timer Cancelled");
            }
        }

        public void StartStopwatch()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public void StopStopwatch()
        {
            stopwatch.Stop();
        }

        public void ResetStopwatch()
        {
            stopwatch.Reset();
        }
    }

}
