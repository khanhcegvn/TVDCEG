using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVDCEG.LBR
{
    public static class PlanarfaceHelper
    {

        public static XYZ Meshvector(this PlanarFace planarFace)
        {
            Mesh mesh = planarFace.Triangulate();
            var fg = mesh.Vertices;
            var p1 = fg[0];
            var p2 = fg[1];
            var vector = p1 - p2;
            return vector;
        }
        public static XYZ CheckComputeNormal(this PlanarFace planarFace,Transform transform)
        {
            XYZ p1 = planarFace.ComputeNormal(UV.Zero);
            XYZ p1project = transform.OfVector(p1).Normalize();
            return p1project;
        }
    }
}
