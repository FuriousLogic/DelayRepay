//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DelayRepay_BL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Station
    {
        public Station()
        {
            this.HomeUsers = new HashSet<User>();
            this.DestinationUsers = new HashSet<User>();
            this.Destinations = new HashSet<Destination>();
            this.FromStations = new HashSet<FromStation>();
        }
    
        public int Id { get; set; }
        public string StationCode { get; set; }
        public string StationName { get; set; }
    
        public virtual ICollection<User> HomeUsers { get; set; }
        public virtual ICollection<User> DestinationUsers { get; set; }
        public virtual ICollection<Destination> Destinations { get; set; }
        public virtual ICollection<FromStation> FromStations { get; set; }
    }
}