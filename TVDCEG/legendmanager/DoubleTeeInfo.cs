using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using TVDCEG.LBR;

namespace TVDCEG
{
	public class DoubleTeeInfo
	{
		public XYZ MarkLP { get; set; }
		public XYZ MarkP { get; set; }
		public XYZ MarkRP { get; set; }
		public XYZ OppLP { get; set; }
		public XYZ OppP { get; set; }	
		public XYZ OppRP { get; set; }
		public List<XYZ> ListPoint { get; set; }
		public FamilyInstance Parent { get; set; }
		private readonly Document document;

		private Autodesk.Revit.DB.View _activeView;

		private Plane _plane;
		public DoubleTeeInfo(Document doc, FamilyInstance familyInstance)
		{
			this.Parent = familyInstance;
			this.document = doc;
			this._activeView = doc.ActiveView;
			this._plane = Plane.CreateByNormalAndOrigin(this._activeView.ViewDirection, this._activeView.Origin);
			this.CalculatePoint();
		}
		private void CalculatePoint()
		{
			try
			{
				Transform transform = this.Parent.GetTransform();
				this.MarkP = transform.OfPoint(XYZ.Zero).ProjectOnto(this._plane);
				Parameter parameter = this.Parent.Symbol.LookupParameter("DT_Stem_Spacing_Form");
				if(parameter!=null)
				{
					double num = parameter.AsDouble();
					Parameter parameter2 = this.Parent.LookupParameter("Flange_Edge_Offset_Left");
					double num2 = parameter2.AsDouble();
					this.MarkLP = transform.OfPoint(new XYZ(num2 + 0.5 * num, 0.0, 0.0)).ProjectOnto(this._plane);
					Parameter parameter3 = this.Parent.LookupParameter("Flange_Edge_Offset_Right");
					double num3 = parameter3.AsDouble();
					this.MarkRP = transform.OfPoint(new XYZ(-num3 - 0.5 * num, 0.0, 0.0)).ProjectOnto(this._plane);
					Parameter parameter4 = this.Parent.LookupParameter("DIM_LENGTH");
					double num4 = parameter4.AsDouble();
					this.OppP = transform.OfPoint(new XYZ(0.0, -num4, 0.0)).ProjectOnto(this._plane);
					this.OppLP = transform.OfPoint(new XYZ(num2 + 0.5 * num, -num4, 0.0)).ProjectOnto(this._plane);
					this.OppRP = transform.OfPoint(new XYZ(-num3 - 0.5 * num, -num4, 0.0)).ProjectOnto(this._plane);
				}
				else
				{
					Parameter parameter2 = this.Parent.LookupParameter("Flange_Edge_Offset_Left");
					double num2 = parameter2.AsDouble();
					this.MarkLP = transform.OfPoint(new XYZ(num2, 0.0, 0.0)).ProjectOnto(this._plane);
					Parameter parameter3 = this.Parent.LookupParameter("Flange_Edge_Offset_Right");
					double num3 = parameter3.AsDouble();
					this.MarkRP = transform.OfPoint(new XYZ(-num3, 0.0, 0.0)).ProjectOnto(this._plane);
					Parameter parameter4 = this.Parent.LookupParameter("DIM_LENGTH");
					double num4 = parameter4.AsDouble();
					this.OppP = transform.OfPoint(new XYZ(0.0, -num4, 0.0)).ProjectOnto(this._plane);
					this.OppLP = transform.OfPoint(new XYZ(num2, -num4, 0.0)).ProjectOnto(this._plane);
					this.OppRP = transform.OfPoint(new XYZ(-num3, -num4, 0.0)).ProjectOnto(this._plane);
				}
				this.ListPoint = new List<XYZ>
				{
					this.MarkLP,
					this.MarkP,
					this.MarkRP,
					this.OppLP,
					this.OppP,
					this.OppRP
				};
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Warning..", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
	
	}
}
