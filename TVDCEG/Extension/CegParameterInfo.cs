using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVDCEG.Extension
{
	public class CegParameterInfo
	{
		public CegParameterInfo()
		{
		}

		public CegParameterInfo(Parameter parameter)
		{
			this.Name = parameter.Definition.Name;
			this.Type = parameter.StorageType;
			this.AsDouble = parameter.AsDouble();
			this.AsInteger = parameter.AsInteger();
			this.AsString = parameter.AsString();
			ElementId elementId = parameter.AsElementId();
			this.Id = ((elementId != null) ? elementId.IntegerValue : -1);
		}
		public string Name { get; set; }
		public StorageType Type { get; set; }
		public double AsDouble { get; set; }
		public int AsInteger { get; set; }
		public string AsString { get; set; }
		public int Id { get; set; }

		public override string ToString()
		{
			string result;
			switch (this.Type)
			{
				case StorageType.Integer:
					result = this.AsInteger.ToString();
					break;
				case StorageType.Double:
					result = this.AsDouble.ToString();
					break;
				case StorageType.String:
					result = this.AsString;
					break;
				default:
					result = "";
					break;
			}
			return result;
		}
	}
}
