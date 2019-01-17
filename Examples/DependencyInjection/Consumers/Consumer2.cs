using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInjection.Services;

namespace DependencyInjection.Consumers
{
    class Consumer2
    {
        private readonly SecondService service;

        public Consumer2(SecondService service)
        {
            this.service = service;
        }

        public void Run()
        {
            this.service.Write();
        }
    }
}
