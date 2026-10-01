using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.MetaData.Enum
{
	internal enum ESendType
	{
		NONE = 0,
		UNICAST = 1,
		BROADCAST = 2,
		BROADCASTEXCEPT = 3
	}
}
