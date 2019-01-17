using System;

namespace DependencyInjection.Services
{
    internal class SecondService
    {
        public DateTime Date { get; set; }

        public SecondService()
        {
            this.Date = DateTime.Now;
        }

        public void Write()
        {
            Console.WriteLine($"SecondService created on {this.Date:HH:mm:ss.fff}");
        }
    }
}