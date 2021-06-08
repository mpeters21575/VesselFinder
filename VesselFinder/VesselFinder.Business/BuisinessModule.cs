using Autofac;
using VesselFinder.Business.Mapper;
using VesselFinder.Business.Models;
using VesselFinder.Business.ServiceClient;

namespace VesselFinder.Business
{
    public class BuisinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VesselFinderHttpClient>().As<IVesselFinderHttpClient>();
            builder.RegisterType<DescriptionAttributeMapper<VesselDetails>>()
                .As<IDescriptionAttributeMapper<VesselDetails>>();
        }
    }
}