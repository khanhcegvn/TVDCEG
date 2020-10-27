using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using TVDCEG.LBR;

namespace TVDCEG
{
	public class DoubleTeeWarpController : RvtController
	{
		public Vertex TL { get; set; } = new Vertex();
		
		public Vertex TR { get; set; } = new Vertex();
		
		public Vertex BL { get; set; } = new Vertex();

		public Vertex BR { get; set; } = new Vertex();

		public List<DoubleTeeInfo> ListDoubleTeeInfos { get; set; } = new List<DoubleTeeInfo>();

		public DoubleTeeWarpController(Document doc) : base(doc)
		{
		}
		
		public override void ReadData()
		{
			IEnumerable<FamilyInstance> enumerable = (from x in base.RSelection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element)
			select base.Doc.GetElement(x)).Cast<FamilyInstance>();
			foreach (FamilyInstance familyInstance in enumerable)
			{
				this.ListDoubleTeeInfos.Add(new DoubleTeeInfo(base.Doc, familyInstance));
			}
			bool flag = !this.CheckTeesParallel();
			if (flag)
			{
				MessageBox.Show("All double tee must be parallel!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			this.Vertices = new List<Vertex>
			{
				this.TL,
				this.TR,
				this.BL,
				this.BR
			};
			this._plane = Plane.CreateByNormalAndOrigin(base.ActiveView.ViewDirection, base.ActiveView.Origin);
			this.GetVertexData();
		}

		public override void Execute()
		{
			this.CaculateValueOfParamter();
		}

		private void CaculateValueOfParamter()
		{
			try
			{
				this.Vertices = (from x in this.Vertices
				orderby x.Z
				select x).ToList<Vertex>();
				Vertex vertex = this.Vertices[1];
				Vertex vertex2 = this.Vertices[2];
				Vertex vertex3 = this.Vertices[3];
				Vertex vertex4 = this.Vertices[0];
				Vertex vertex5 = null;
				Vertex vertex6 = null;
				Vertex vertex7 = null;
				DoubleTeeInfo doubleTeeInfo = vertex4.DoubleTeeInfo;
				XYZ q = doubleTeeInfo.MarkP - doubleTeeInfo.OppP;
				double compareTo = vertex4.Point.DotProduct(base.ActiveView.UpDirection);
				double value = vertex.Point.DotProduct(base.ActiveView.UpDirection);
				double value2 = vertex2.Point.DotProduct(base.ActiveView.UpDirection);
				double value3 = vertex3.Point.DotProduct(base.ActiveView.UpDirection);
				bool flag = value.EQ(compareTo, 0.01);
				if (flag)
				{
					vertex5 = vertex;
				}
				bool flag2 = value2.EQ(compareTo, 0.01);
				if (flag2)
				{
					vertex5 = vertex2;
				}
				bool flag3 = value3.EQ(compareTo, 0.01);
				if (flag3)
				{
					vertex5 = vertex3;
				}
				// sua loi 
				else
				{
					vertex5 = vertex3;
				}
				foreach (Vertex vertex8 in this.Vertices)
				{
					bool flag4 = vertex8.Point.IsAlmostEqualTo(vertex4.Point) || vertex8.Point.IsAlmostEqualTo(vertex5.Point);
					if (!flag4)
					{
						XYZ p = vertex4.Point - vertex8.Point;
						XYZ p2 = vertex5.Point - vertex8.Point;
						// can sua lai
						bool flag5 = p.IsParallel(q);
						if (flag5)
						{
							vertex6 = vertex8;
						}
						bool flag6 = p2.IsParallel(q);
						if (flag6)
						{
							vertex7 = vertex8;
						}
					}
				}
				XYZ xyz = vertex4.Point - vertex.Point;
				Vertex vertex9 = this.Vertices.Last<Vertex>();
				double z = vertex5.Z;
				double z2 = vertex4.Z;
				double num = Math.Abs((vertex4.Point - vertex5.Point).DotProduct(base.ActiveView.RightDirection));
				XYZ point = vertex5.Point;
				List<TeeParamterValues> list = new List<TeeParamterValues>();
				foreach (DoubleTeeInfo doubleTeeInfo2 in this.ListDoubleTeeInfos)
				{
					TeeParamterValues teeParamterValues = new TeeParamterValues
					{
						Tee = doubleTeeInfo2.Parent,
						Length_of_Drop = num,
						High_Point_Elevation = z,
						Low_Point_Elevation = z2
					};
					teeParamterValues.CL_DT = Math.Abs((point - doubleTeeInfo2.MarkP).DotProduct(base.RightDirection));
					double num2 = point.DistanceTo(doubleTeeInfo2.MarkP);
					double num3 = point.DistanceTo(doubleTeeInfo2.OppP);
					bool flag7 = num2 < num3;
					if (flag7)
					{
						teeParamterValues.Warp_Mark_End = true;
					}
					else
					{
						teeParamterValues.Warp_Mark_End = false;
					}
					double num4 = point.DistanceTo(doubleTeeInfo2.MarkLP);
					double num5 = point.DistanceTo(doubleTeeInfo2.MarkRP);
					bool flag8 = num4 < num5;
					if (flag8)
					{
						teeParamterValues.Hign_Point_Right = 0;
					}
					else
					{
						teeParamterValues.Hign_Point_Right = 1;
					}
					XYZ point2 = vertex7.Point;
					bool flag9 = !vertex6.Z.EQ(vertex7.Z);
					if (flag9)
					{
						double num6 = Math.Abs(vertex6.Z);
						double num7 = Math.Atan(num6 / num);
						double num8 = 0.0;
						bool flag10 = vertex7.Z != 0.0;
						if (flag10)
						{
							num8 = Math.Abs(vertex7.Z * num / Math.Abs(Math.Abs(vertex6.Z) - Math.Abs(vertex7.Z)));
							num7 = Math.Atan(Math.Abs(vertex7.Z) / num8);
						}
						double num9 = Math.Abs(point2.DistanceTo(doubleTeeInfo2.MarkLP));
						double num10 = Math.Abs(point2.DistanceTo(doubleTeeInfo2.MarkRP));
						double num11 = Math.Abs(point2.DistanceTo(doubleTeeInfo2.MarkP));
						double num12 = Math.Abs(point2.DistanceTo(doubleTeeInfo2.OppP));
						bool flag11 = num11 < num12;
						if (flag11)
						{
							bool flag12 = num9 < num10;
							double manual_Mark_End_Warp_Angle;
							if (flag12)
							{
								manual_Mark_End_Warp_Angle = num7;
							}
							else
							{
								manual_Mark_End_Warp_Angle = -num7;
							}
							teeParamterValues.Manual_Mark_End_Warp_Angle = manual_Mark_End_Warp_Angle;
							teeParamterValues.Manual_Mark_End_Offset = -Math.Abs((teeParamterValues.CL_DT + num8) * Math.Tan(num7));
						}
						else
						{
							bool flag13 = num9 < num10;
							double manual_Opp_End_Warp_Angle;
							if (flag13)
							{
								manual_Opp_End_Warp_Angle = num7;
							}
							else
							{
								manual_Opp_End_Warp_Angle = -num7;
							}
							teeParamterValues.Manual_Opp_End_Warp_Angle = manual_Opp_End_Warp_Angle;
							teeParamterValues.Manual_Opp_End_Offset = -Math.Abs((teeParamterValues.CL_DT + num8) * Math.Tan(num7));
						}
					}
					else
					{
						double num13 = Math.Abs(point2.DistanceTo(doubleTeeInfo2.MarkLP));
						double num14 = Math.Abs(point2.DistanceTo(doubleTeeInfo2.MarkRP));
						double num15 = Math.Abs(point2.DistanceTo(doubleTeeInfo2.MarkP));
						double num16 = Math.Abs(point2.DistanceTo(doubleTeeInfo2.OppP));
						bool flag14 = num15 < num16;
						if (flag14)
						{
							teeParamterValues.Manual_Mark_End_Warp_Angle = 0.0;
							teeParamterValues.Manual_Mark_End_Offset = vertex6.Z;
						}
						else
						{
							teeParamterValues.Manual_Opp_End_Warp_Angle = 0.0;
							teeParamterValues.Manual_Opp_End_Offset = vertex6.Z;
						}
					}
					list.Add(teeParamterValues);
				}
				foreach (TeeParamterValues teeParamterValues2 in list)
				{
					FamilyInstance tee = teeParamterValues2.Tee;
					Parameter parameter = tee.LookupParameter("CL_DT");
					parameter.Set(teeParamterValues2.CL_DT);
					Parameter parameter2 = tee.LookupParameter(TeeParamters.DropLength);
					parameter2.Set(teeParamterValues2.Length_of_Drop);
					Parameter parameter3 = tee.LookupParameter(TeeParamters.HignPointElevation);
					parameter3.Set(teeParamterValues2.High_Point_Elevation);
					Parameter parameter4 = tee.LookupParameter(TeeParamters.LowPointElevation);
					parameter4.Set(teeParamterValues2.Low_Point_Elevation);
					Parameter parameter5 = tee.LookupParameter(TeeParamters.MarkEnd);
					Parameter parameter6 = tee.LookupParameter(TeeParamters.OppEnd);
					bool warp_Mark_End = teeParamterValues2.Warp_Mark_End;
					if (warp_Mark_End)
					{
						parameter5.Set(1);
						parameter6.Set(0);
					}
					else
					{
						parameter5.Set(0);
						parameter6.Set(1);
					}
					Parameter parameter7 = tee.LookupParameter(TeeParamters.HignPointRight);
					parameter7.Set(teeParamterValues2.Hign_Point_Right);
					Parameter parameter8 = tee.LookupParameter(TeeParamters.Manual_Mark_End_Offset);
					parameter8.Set(teeParamterValues2.Manual_Mark_End_Offset);
					Parameter parameter9 = tee.LookupParameter(TeeParamters.Manual_Mark_End_Warp_Angle);
					parameter9.Set(teeParamterValues2.Manual_Mark_End_Warp_Angle);
					Parameter parameter10 = tee.LookupParameter(TeeParamters.Manual_Opp_End_Offset);
					parameter10.Set(teeParamterValues2.Manual_Opp_End_Offset);
					Parameter parameter11 = tee.LookupParameter(TeeParamters.Manual_Opp_End_Warp_Angle);
					parameter11.Set(teeParamterValues2.Manual_Opp_End_Warp_Angle);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Warning..", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		public void GetVertexData()
		{
			IOrderedEnumerable<DoubleTeeInfo> source = from x in this.ListDoubleTeeInfos
			orderby x.MarkP.DotProduct(base.RightDirection)
			select x;
			DoubleTeeInfo doubleTeeInfo = source.First<DoubleTeeInfo>();
			DoubleTeeInfo doubleTeeInfo2 = source.Last<DoubleTeeInfo>();
			this.TL.DoubleTeeInfo = doubleTeeInfo;
			this.BL.DoubleTeeInfo = doubleTeeInfo;
			List<XYZ> list = (from x in doubleTeeInfo.ListPoint
			orderby x.DotProduct(base.RightDirection)
			select x).ToList<XYZ>();
			XYZ xyz = list[0].ProjectOnto(this._plane);
			XYZ xyz2 = list[1].ProjectOnto(this._plane);
			double num = xyz.DotProduct(base.Updirection);
			double num2 = xyz2.DotProduct(base.Updirection);
			bool flag = num < num2;
			if (flag)
			{
				this.TL.Point = xyz2;
				this.BL.Point = xyz;
			}
			else
			{
				this.TL.Point = xyz;
				this.BL.Point = xyz2;
			}
			this.TR.DoubleTeeInfo = doubleTeeInfo2;
			this.BR.DoubleTeeInfo = doubleTeeInfo2;
			List<XYZ> list2 = (from x in doubleTeeInfo2.ListPoint
			orderby x.DotProduct(-base.RightDirection)
			select x).ToList<XYZ>();
			XYZ xyz3 = list2[0].ProjectOnto(this._plane);
			XYZ xyz4 = list2[1].ProjectOnto(this._plane);
			double num3 = xyz3.DotProduct(base.Updirection);
			double num4 = xyz4.DotProduct(base.Updirection);
			bool flag2 = num3 < num4;
			if (flag2)
			{
				this.TR.Point = xyz4;
				this.BR.Point = xyz3;
			}
			else
			{
				this.TR.Point = xyz3;
				this.BR.Point = xyz4;
			}
		}

		private bool CheckTeesParallel()
		{
			bool result = true;
			XYZ q = (this.ListDoubleTeeInfos[0].MarkP - this.ListDoubleTeeInfos[0].OppP).Normalize();
			foreach (DoubleTeeInfo doubleTeeInfo in this.ListDoubleTeeInfos)
			{
				XYZ p = (doubleTeeInfo.MarkP - doubleTeeInfo.OppP).Normalize();
				bool flag = !p.IsParallel(q);
				if (flag)
				{
					result = false;
				}
			}
			return result;
		}

		private List<Vertex> Vertices;

		private Plane _plane;
	}
	public class TeeParamterValues
	{
		public FamilyInstance Tee { get; set; }
		public double CL_DT { get; set; } = 0;
		public double Length_of_Drop { get; set; } = 0;
		public double High_Point_Elevation { get; set; } = 0;
		public double Low_Point_Elevation { get; set; } = 0;
		public bool Warp_Mark_End { get; set; }
		public int Hign_Point_Right { get; set; }
		public double Manual_Mark_End_Offset { get; set; } = 0;
		public double Manual_Mark_End_Warp_Angle { get; set; } = 0;
		public double Manual_Opp_End_Offset { get; set; } = 0;
		public double Manual_Opp_End_Warp_Angle { get; set; } = 0;
	}
	public class Vertex
	{
		public double Z { get; set; } = 0;
		public DoubleTeeInfo DoubleTeeInfo { get; set; }
		public XYZ Point;
	}
}
