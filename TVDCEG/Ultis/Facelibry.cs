using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using TVDCEG.LBR;

namespace TVDCEG.Ultis
{
	public class Facelibry
	{
		public static PlanarFace GetPickedFace(Document doc, Selection selection, string message, out Element pickedElement, out Reference reference)
		{
			Options options = new Options();
			options.ComputeReferences = true;
			reference = selection.PickObject(ObjectType.Face, message);
			pickedElement = doc.GetElement(reference);
			GeometryObject geometryObjectFromReference = pickedElement.GetGeometryObjectFromReference(reference);
			return geometryObjectFromReference as PlanarFace;
		}

		public static bool IsPerpendicular(PlanarFace face, XYZ vector)
		{
			bool result = false;
			XYZ faceNormal = face.FaceNormal;
			XYZ xyz = vector.CrossProduct(faceNormal) / (vector.GetLength() * faceNormal.GetLength());
			bool flag = xyz.GetLength() < 1E-05;
			if (flag)
			{
				result = true;
			}
			return result;
		}

		public static bool IsPerpendicular(PlanarFace face, XYZ vector, Transform transform)
		{
			bool result = false;
			XYZ xyz = face.FaceNormal;
			xyz = transform.OfVector(xyz);
			XYZ xyz2 = vector.CrossProduct(xyz) / (vector.GetLength() * xyz.GetLength());
			bool flag = xyz2.GetLength() < 0.001;
			if (flag)
			{
				result = true;
			}
			return result;
		}

		public static XYZ CenterPoint(PlanarFace face)
		{
			XYZ xyz = new XYZ();
			Mesh mesh = face.Triangulate();
			IList<XYZ> list = new List<XYZ>();
			foreach (XYZ xyz2 in mesh.Vertices)
			{
				xyz += xyz2;
				list.Add(xyz2);
			}
			xyz /= (double)list.Count;
			return xyz;
		}

		public static IList<PlanarFace> PerpendicularFace(IList<PlanarFace> listFaces, XYZ vector)
		{
			IList<PlanarFace> list = new List<PlanarFace>();
			foreach (PlanarFace planarFace in listFaces)
			{
				bool flag = Facelibry.IsPerpendicular(planarFace, vector);
				if (flag)
				{
					list.Add(planarFace);
				}
			}
			return list;
		}
		public static IList<PlanarFace> PerpendicularFace(IList<PlanarFace> listFaces, XYZ vector, Transform transform)
		{
			IList<PlanarFace> list = new List<PlanarFace>();
			foreach (PlanarFace planarFace in listFaces)
			{
				bool flag = Facelibry.IsPerpendicular(planarFace, vector, transform);
				if (flag)
				{
					list.Add(planarFace);
				}
			}
			return list;
		}

		public static PlanarFace FirstFace(IList<PlanarFace> listFace, XYZ vector)
		{
			PlanarFace result = null;
			IList<PlanarFace> list = Facelibry.PerpendicularFace(listFace, vector);
			double num = 0.0;
			foreach (PlanarFace planarFace in list)
			{
				XYZ source = Facelibry.CenterPoint(planarFace);
				bool flag = MathLib.IsEqual(num, 0.0, 0.0001);
				if (flag)
				{
					num = vector.DotProduct(source);
					result = planarFace;
				}
				else
				{
					double num2 = vector.DotProduct(source);
					bool flag2 = num2 < num;
					if (flag2)
					{
						result = planarFace;
						num = num2;
					}
				}
			}
			return result;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00004EF0 File Offset: 0x000030F0
		public static PlanarFace FirstFace(IList<PlanarFace> listFace, XYZ vector, Transform transform)
		{
			PlanarFace result = null;
			IList<PlanarFace> list = Facelibry.PerpendicularFace(listFace, vector, transform);
			double num = 0.0;
			foreach (PlanarFace planarFace in list)
			{
				XYZ xyz = Facelibry.CenterPoint(planarFace);
				xyz = transform.OfPoint(xyz);
				bool flag = MathLib.IsEqual(num, 0.0, 0.0001);
				if (flag)
				{
					num = vector.DotProduct(xyz);
					result = planarFace;
				}
				else
				{
					double num2 = vector.DotProduct(xyz);
					bool flag2 = num2 < num;
					if (flag2)
					{
						result = planarFace;
						num = num2;
					}
				}
			}
			return result;
		}

		public static PlanarFace LastFace(IList<PlanarFace> listFace, XYZ vector)
		{
			PlanarFace result = null;
			IList<PlanarFace> list = Facelibry.PerpendicularFace(listFace, vector);
			double num = 0.0;
			foreach (PlanarFace planarFace in list)
			{
				XYZ origin = planarFace.Origin;
				bool flag = MathLib.IsEqual(num, 0.0, 0.0001);
				if (flag)
				{
					num = vector.DotProduct(origin);
					result = planarFace;
				}
				else
				{
					double num2 = vector.DotProduct(origin);
					bool flag2 = num2 > num;
					if (flag2)
					{
						result = planarFace;
						num = num2;
					}
				}
			}
			return result;
		}

		public static PlanarFace LastFace(IList<PlanarFace> listFace, XYZ vector, Transform transform)
		{
			PlanarFace result = null;
			IList<PlanarFace> list = Facelibry.PerpendicularFace(listFace, vector, transform);
			double num = 0.0;
			foreach (PlanarFace planarFace in list)
			{
				XYZ xyz = planarFace.Origin;
				xyz = transform.OfPoint(xyz);
				bool flag = MathLib.IsEqual(num, 0.0, 0.0001);
				if (flag)
				{
					num = vector.DotProduct(xyz);
					result = planarFace;
				}
				else
				{
					double num2 = vector.DotProduct(xyz);
					bool flag2 = num2 > num;
					if (flag2)
					{
						result = planarFace;
						num = num2;
					}
				}
			}
			return result;
		}

		public static PlanarFace NearestFace(PlanarFace face, IList<PlanarFace> listFaces)
		{
			PlanarFace planarFace = null;
			double num = 0.0;
			XYZ faceNormal = face.FaceNormal;
			XYZ right = Facelibry.CenterPoint(face);
			foreach (PlanarFace planarFace2 in listFaces)
			{
				XYZ faceNormal2 = planarFace2.FaceNormal;
				XYZ xyz = faceNormal.CrossProduct(faceNormal2) / (faceNormal.GetLength() * faceNormal2.GetLength());
				bool flag = !xyz.IsAlmostEqualTo(XYZ.Zero);
				if (!flag)
				{
					XYZ xyz2 = Facelibry.CenterPoint(planarFace2);
					XYZ xyz3 = xyz2 - right;
					bool flag2 = xyz3.IsAlmostEqualTo(new XYZ());
					if (!flag2)
					{
						double num2 = Facelibry.PointToFace(xyz2, face);
						bool flag3 = null == planarFace && num2 > 1E-06;
						if (flag3)
						{
							planarFace = planarFace2;
							num = num2;
						}
						else
						{
							bool flag4 = num2 < num && num2 > 1E-07;
							if (flag4)
							{
								planarFace = planarFace2;
								num = num2;
							}
						}
					}
				}
			}
			return planarFace;
		}

		public static IList<PlanarFace> ListNearestFace(PlanarFace face, IList<PlanarFace> listFaces)
		{
			IList<PlanarFace> list = new List<PlanarFace>();
			PlanarFace face2 = Facelibry.NearestFace(face, listFaces);
			XYZ point = Facelibry.CenterPoint(face2);
			double num = Facelibry.PointToFace(point, face);
			XYZ faceNormal = face.FaceNormal;
			XYZ right = Facelibry.CenterPoint(face);
			foreach (PlanarFace planarFace in listFaces)
			{
				XYZ xyz = Facelibry.CenterPoint(planarFace);
				XYZ xyz2 = xyz - right;
				bool flag = xyz2.IsAlmostEqualTo(new XYZ());
				if (!flag)
				{
					double num2 = Facelibry.PointToFace(xyz, face);
					bool flag2 = num2 < 1.02 * num && num2 > 0.98 * num;
					if (flag2)
					{
						list.Add(planarFace);
					}
				}
			}
			return list;
		}

		public static PlanarFace NearestFace(PlanarFace face, IList<PlanarFace> listFaces, Transform transform)
		{
			PlanarFace planarFace = null;
			double num = 0.0;
			XYZ xyz = transform.OfVector(face.FaceNormal);
			XYZ right = transform.OfPoint(face.Origin);
			foreach (PlanarFace planarFace2 in listFaces)
			{
				XYZ source = transform.OfVector(planarFace2.FaceNormal);
				XYZ xyz2 = xyz.CrossProduct(source);
				bool flag = !xyz2.IsAlmostEqualTo(XYZ.Zero);
				if (!flag)
				{
					XYZ left = transform.OfPoint(planarFace2.Origin);
					XYZ xyz3 = left - right;
					bool flag2 = xyz3.IsAlmostEqualTo(XYZ.Zero);
					if (!flag2)
					{
						double num2 = Math.Abs(xyz3.DotProduct(xyz) / xyz.GetLength());
						bool flag3 = null == planarFace;
						if (flag3)
						{
							planarFace = planarFace2;
							num = num2;
						}
						else
						{
							bool flag4 = num2 < num;
							if (flag4)
							{
								planarFace = planarFace2;
								num = num2;
							}
						}
					}
				}
			}
			return planarFace;
		}
		public static PlanarFace NearestFace(PlanarFace face, IList<PlanarFace> listFaces, IList<PlanarFace> exceptFaces, Transform transform)
		{
			PlanarFace planarFace = null;
			double num = 0.0;
			XYZ xyz = transform.OfVector(face.FaceNormal);
			XYZ right = transform.OfPoint(face.Origin);
			foreach (PlanarFace planarFace2 in listFaces)
			{
				bool flag = exceptFaces.Contains(planarFace2);
				if (!flag)
				{
					XYZ source = transform.OfVector(planarFace2.FaceNormal);
					XYZ xyz2 = xyz.CrossProduct(source);
					bool flag2 = !xyz2.IsAlmostEqualTo(XYZ.Zero);
					if (!flag2)
					{
						XYZ left = transform.OfPoint(planarFace2.Origin);
						XYZ xyz3 = left - right;
						bool flag3 = xyz3.IsAlmostEqualTo(XYZ.Zero);
						if (!flag3)
						{
							double num2 = Math.Abs(xyz3.DotProduct(xyz) / xyz.GetLength());
							bool flag4 = null == planarFace;
							if (flag4)
							{
								planarFace = planarFace2;
								num = num2;
							}
							else
							{
								bool flag5 = num2 < num;
								if (flag5)
								{
									planarFace = planarFace2;
									num = num2;
								}
							}
						}
					}
				}
			}
			return planarFace;
		}

		public static PlanarFace FurthestFace(PlanarFace face, IList<PlanarFace> listFaces, Transform transform)
		{
			PlanarFace result = null;
			double num = double.MinValue;
			XYZ xyz = transform.OfVector(face.FaceNormal);
			XYZ xyz2 = transform.OfPoint(face.Origin);
			PLane3D plane3DLib = new PLane3D(transform.OfPoint(face.Origin), transform.OfVector(face.FaceNormal));
			foreach (PlanarFace planarFace in listFaces)
			{
				XYZ source = transform.OfVector(planarFace.FaceNormal);
				XYZ xyz3 = xyz.CrossProduct(source);
				bool flag = xyz3.GetLength() > 0.001;
				if (!flag)
				{
					double num2 = plane3DLib.DistanceFromPointToPlane(transform.OfPoint(planarFace.Origin));
					bool flag2 = num2 < 0.001;
					if (!flag2)
					{
						bool flag3 = num2 > num;
						if (flag3)
						{
							result = planarFace;
							num = num2;
						}
					}
				}
			}
			return result;
		}

		public static PlanarFace FurthestFace(XYZ point, XYZ direction, IList<PlanarFace> listFaces, Transform transform)
		{
			PlanarFace result = null;
			double num = double.MinValue;
			PLane3D plane3DLib = new PLane3D(point, direction);
			foreach (PlanarFace planarFace in listFaces)
			{
				XYZ xyz = transform.OfVector(planarFace.FaceNormal);
				XYZ xyz2 = direction.CrossProduct(xyz);
				bool flag = xyz2.GetLength() > 0.001 || xyz.DotProduct(direction) < 0.0;
				if (!flag)
				{
					double num2 = plane3DLib.DistanceFromPointToPlane(transform.OfPoint(planarFace.Origin));
					bool flag2 = num2 < 0.001;
					if (!flag2)
					{
						bool flag3 = num2 > num;
						if (flag3)
						{
							result = planarFace;
							num = num2;
						}
					}
				}
			}
			return result;
		}

		public static double TwoFaceDistance(PlanarFace face1, PlanarFace face2)
		{
			double result = -1.0;
			XYZ xyz = face1.FaceNormal;
			xyz = XYZHelper.AbsVector(xyz);
			XYZ xyz2 = face2.FaceNormal;
			xyz2 = XYZHelper.AbsVector(xyz2);
			bool flag = xyz.IsAlmostEqualTo(xyz2);
			if (flag)
			{
				XYZ xyz3 = face1.Origin - face2.Origin;
				result = Math.Abs(xyz3.DotProduct(xyz) / xyz.GetLength());
			}
			return result;
		}

		public static double TwoFaceDistance(PlanarFace face1, Transform transform1, PlanarFace face2, Transform transform2)
		{
			double result = -1.0;
			XYZ xyz = transform1.OfVector(face1.FaceNormal);
			xyz = XYZHelper.AbsVector(xyz);
			XYZ xyz2 = transform2.OfVector(face2.FaceNormal);
			xyz2 = XYZHelper.AbsVector(xyz2);
			bool flag = xyz.IsAlmostEqualTo(xyz2);
			if (flag)
			{
				XYZ left = transform1.OfPoint(face1.Origin);
				XYZ right = transform2.OfPoint(face2.Origin);
				XYZ xyz3 = left - right;
				result = Math.Abs(xyz3.DotProduct(xyz) / xyz.GetLength());
			}
			return result;
		}

		public static double PointToFace(XYZ point, PlanarFace face)
		{
			XYZ faceNormal = face.FaceNormal;
			XYZ xyz = point - face.Origin;
			return Math.Abs(xyz.DotProduct(faceNormal) / faceNormal.GetLength());
		}

		public static double PointToFace(XYZ point, PlanarFace face, Transform transform)
		{
			XYZ normal = transform.OfVector(face.FaceNormal);
			XYZ origin = transform.OfPoint(face.Origin);
			PLane3D plane3DLib = new PLane3D(origin, normal);
			return plane3DLib.DistanceFromPointToPlane(point);
		}

		public static CurveElement NearestLinesToFace(PlanarFace face, IList<CurveElement> listLines)
		{
			CurveElement curveElement = null;
			double num = 0.0;
			foreach (CurveElement curveElement2 in listLines)
			{
				DetailLine detailLine = curveElement2 as DetailLine;
				bool flag = detailLine == null;
				if (!flag)
				{
					XYZ endPoint = detailLine.GeometryCurve.GetEndPoint(0);
					double num2 = Facelibry.PointToFace(endPoint, face);
					bool flag2 = curveElement == null;
					if (flag2)
					{
						num = num2;
						curveElement = curveElement2;
					}
					else
					{
						bool flag3 = num2 < num;
						if (flag3)
						{
							num = num2;
							curveElement = curveElement2;
						}
					}
				}
			}
			return curveElement;
		}

		public static PlanarFace GetTopFace(IList<PlanarFace> faces, XYZ direction, Transform transform)
		{
			PlanarFace result = null;
			double num = double.MinValue;
			XYZ point = new XYZ();
			double num2 = double.MinValue;
			foreach (PlanarFace planarFace in faces)
			{
				foreach (object obj in planarFace.EdgeLoops)
				{
					EdgeArray edgeArray = (EdgeArray)obj;
					foreach (object obj2 in edgeArray)
					{
						Edge edge = (Edge)obj2;
						Curve curve = edge.AsCurve();
						bool flag = curve == null;
						if (!flag)
						{
							XYZ xyz = transform.OfPoint(curve.GetEndPoint(0));
							XYZ xyz2 = transform.OfPoint(curve.GetEndPoint(1));
							double num3 = xyz.DotProduct(direction);
							double num4 = xyz2.DotProduct(direction);
							bool flag2 = num3 > num2;
							if (flag2)
							{
								point = xyz;
								num2 = num3;
							}
							bool flag3 = num4 > num2;
							if (flag3)
							{
								point = xyz2;
								num2 = num4;
							}
						}
					}
				}
			}
			List<PlanarFace> list = new List<PlanarFace>();
			foreach (PlanarFace planarFace2 in faces)
			{
				double num5 = Facelibry.PointToFace(point, planarFace2, transform);
				bool flag4 = num5 < 0.001;
				if (flag4)
				{
					list.Add(planarFace2);
				}
			}
			foreach (PlanarFace planarFace3 in list)
			{
				XYZ xyz3 = transform.OfVector(planarFace3.FaceNormal);
				double num6 = xyz3.DotProduct(direction);
				bool flag5 = num6 > num;
				if (flag5)
				{
					result = planarFace3;
					num = num6;
				}
			}
			return result;
		}
	}
}
