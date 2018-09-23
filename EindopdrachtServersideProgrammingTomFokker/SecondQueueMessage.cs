using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EindopdrachtServersideProgrammingTomFokker
{
    class SecondQueueMessage
    {
        public string blobName { get; set; }
        public OpenWeatherMapResult weather { get; set; }
    }
}
