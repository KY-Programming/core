using System;

namespace First.Module.Services
{
    public interface IFirstServices
    {
        void Write();
    }

    internal class FirstServices : IFirstServices
    {
        public void Write()
        {
            Console.WriteLine("Hello from first service");
        }
    }
}