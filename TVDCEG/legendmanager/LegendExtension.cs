using System;

namespace TVDCEG
{
	public class LegendExtension
	{
		public string Name { get; set; }
		public int Id { get; set; }
		public string Creator { get; set; }
		public string LastChangedBy { get; set; }
		public string ListSheets { get; set; }
		public int MoreInfo { get; set; } = 0;
	}
}
