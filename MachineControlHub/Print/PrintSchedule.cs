using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineControlHub.PrinterConnection;

namespace MachineControlHub.Print
{
    public class PrintSchedule
    {
        private IPrinterConnection Connection { get; set; }

        public PrintSchedule() { }

        public PrintSchedule(IPrinterConnection connection)
        {
            Connection = connection;
        }

        public TimeSpan? ScheduleTime { get; set; }

        public string ScheduleName { get; set; }

        public double ScheduleSize { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public string ScheduleStatus { get; set; }

        public string SchedulePrinter { get; set; }
    }
}