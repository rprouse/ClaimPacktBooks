using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;

namespace ClaimBookFunction
{
    public static class ClaimBook
    {
        [FunctionName("ClaimBook")]
        public static async Task Run([TimerTrigger("0 30 9 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            string user = Environment.GetEnvironmentVariable("PACKT_USER");
            string pass = Environment.GetEnvironmentVariable("PACKT_PASS");

            if(string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                log.Error("PACKT_USER and/or PACKT_PASS environment variables have not been set.");
                return;
            }

            log.Info($"Fetching latest Packt book at: {DateTime.Now}");

            var packt = new Packt(log);
            await packt.ClaimFreeBook(user, pass);
        }
    }
}