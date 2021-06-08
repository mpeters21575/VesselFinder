using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace VesselFinder.Business.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class VesselDetails
    {
        [Description("Current draught")]
        public string Draught { get; private set; }
        [Description("Navigation Status")]
        public string NavigationStatus { get; private set; }
        [Description("IMO number")]
        public string IMO { get; private set; }
        [Description("Vessel Name")]
        public string VesselName { get; private set; }
        [Description("Length Overall (m)")]
        public string Length { get; private set; }
        [Description("Beam (m)")]
        public string Beam { get; private set; }
        [Description("Ship type")]
        public string ShipType { get; private set; }
        [Description("Gross Tonnage")]
        public string GrossTonnage { get; private set; }
        [Description("Summer Deadweight (t)")]
        public string SummerDeadweight { get; private set; }
        [Description("Year of Built")]
        public string BuildYear { get; private set; }
        [Description("Flag")]
        public string Flag { get; private set; }
    }
}