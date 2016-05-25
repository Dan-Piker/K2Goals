using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class PolygonArea : GoalObject
    {
        public double Strength;
        public double TargetArea;

        public PolygonArea()
        {
        }

        public PolygonArea(List<int> V, double Area, double k)
        {
            int L = V.Count;
            PIndex = V.ToArray();
            Move = new Vector3d[L];
            Weighting = new double[L];
            for (int i = 0; i < L; i++)
            {
                Weighting[i] = k;
            }
            TargetArea = Area;
            Strength = k;
        }

        public PolygonArea(List<Point3d> V, double Area, double k)
        {
            int L = V.Count;
            PPos = V.ToArray();
            Move = new Vector3d[L];
            Weighting = new double[L];
            for (int i = 0; i < L; i++)
            {
                Weighting[i] = k;
            }
            TargetArea = Area;
            Strength = k;
        }
        
        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            double CurrentAreaDoubled = 0;
            double TotalLength = 0;
            int L = PIndex.Length;

            for (int i = 0; i < L; i++)
            {
                CurrentAreaDoubled += Vector3d.CrossProduct(
                  (Vector3d)p[PIndex[i]].Position,
                  (Vector3d)p[PIndex[(i + 1) % L]].Position).Z;
                //note - points must be ordered CCW

                TotalLength += p[PIndex[i]].Position.DistanceTo
                  (p[PIndex[(i + 1) % L]].Position);

                Move[i] = Vector3d.Zero;
            }

            double AreaShortage = TargetArea - 0.5 * CurrentAreaDoubled;
            double Offset = AreaShortage / TotalLength;

            for (int i = 0; i < L; i++)
            {
                int NextVert = (i + 1) % L;
                Vector3d Edge = (p[PIndex[(i + 1) % L]].Position - p[PIndex[i]].Position);
                Edge.Unitize();
                Vector3d Pressure = Offset * Vector3d.CrossProduct(Edge, Vector3d.ZAxis);
                Move[i] += Pressure;
                Move[NextVert] += Pressure;
                Weighting[i] = Strength;
            }
        }       
    }
}
