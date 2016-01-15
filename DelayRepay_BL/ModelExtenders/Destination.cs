using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayRepay_BL
{
    public partial class Destination
    {
        public int MinutesDelay
        {
            get
            {
                TimeSpan ts = this.ActualArrival - this.ScheduledArrival;
                return ts.Minutes;
            }
        }
        public bool IsClaimable
        {
            get { return this.MinutesDelay >= 30; }
        }
    }
}
