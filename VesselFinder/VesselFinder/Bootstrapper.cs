using Autofac;
using VesselFinder.Business;

namespace VesselFinder
{
    public static class Bootstrapper
    {
        public static IContainer Bootup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new BuisinessModule());
            
            return builder.Build();
        }
    }
}