using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Tries to make the 4 vertices of a quad lie on a common circle.
    /// Should be used in conjunction with planarize.
    /// Can be used for generating circular meshes, which have useful offset properties for beam structures.
    /// </summary>
    public class CyclicQuad : GoalObject
    {
        public double Strength;

        public CyclicQuad()
        {
        }

        public CyclicQuad(int[] P, double k)
        {
            PIndex = P;
            Move = new Vector3d[4];
            Weighting = new double[4];
            Strength = k;
        }

        public CyclicQuad(Point3d[] P, double k)
        {            
            PPos = P;
            Move = new Vector3d[4];
            Weighting = new double[4];
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Point3d P1 = p[PIndex[0]].Position;
            Point3d P2 = p[PIndex[1]].Position;
            Point3d P3 = p[PIndex[2]].Position;
            Point3d P4 = p[PIndex[3]].Position;

            Point3d Center =
              (
              (new Circle(P1, P2, P3)).Center +
              (new Circle(P2, P3, P4)).Center +
              (new Circle(P3, P4, P1)).Center +
              (new Circle(P4, P1, P2)).Center
              ) * 0.25;
            double D1 = Center.DistanceTo(P1);
            double D2 = Center.DistanceTo(P2);
            double D3 = Center.DistanceTo(P3);
            double D4 = Center.DistanceTo(P4);
            double AvgDist = 0.25 * (D1 + D2 + D3 + D4);

            double stretchfactor;
            stretchfactor = 1.0 - AvgDist / D1;
            Move[0] = (Center - P1) * stretchfactor;
            stretchfactor = 1.0 - AvgDist / D2;
            Move[1] = (Center - P2) * stretchfactor;
            stretchfactor = 1.0 - AvgDist / D3;
            Move[2] = (Center - P3) * stretchfactor;
            stretchfactor = 1.0 - AvgDist / D4;
            Move[3] = (Center - P4) * stretchfactor;

            Weighting[0] = Strength;
            Weighting[1] = Strength;
            Weighting[2] = Strength;
            Weighting[3] = Strength;
        }
     
    }
}
