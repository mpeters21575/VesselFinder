using System;
using System.Threading.Tasks;
using Autofac;
using VesselFinder.Business.ServiceClient;

namespace VesselFinder
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var imo = args.Length > 0 && !string.IsNullOrWhiteSpace(args[0])
                ? args[0]
                : "9776420"; // testing purposes

            var details = await WithClient().GetVesselDetailsAsync(imo);
            if (details == null)
            {
                Console.WriteLine($"No details found for IMO {imo}");
                return;
            }
            
            Console.WriteLine($"Details found for {details.VesselName} (IMO {details.IMO})");
        }

        private static IVesselFinderHttpClient WithClient()
        {
            var container = Bootstrapper.Bootup();
            var client = container.Resolve<IVesselFinderHttpClient>();
            return client;
        }
    }
}