using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVDCEG.Extension
{
	public class CegParameterSet
	{
		public CegParameterSet()
		{
			this.Parameters = new Dictionary<string, CegParameterInfo>();
		}
		public Dictionary<string, CegParameterInfo> Parameters { get; set; }
	}
}
