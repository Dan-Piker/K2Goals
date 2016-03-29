using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class AnchorXYZ : GoalObject
    {
        public double Strength;
        public Point3d PtA;
        public bool xFix, yFix, zFix;

        public AnchorXYZ(Point3d P, bool X, bool Y, bool Z, double k)
        {
            PPos = new Point3d[1] { P };
            Move = new Vector3d[1];
            Weighting = new double[1] { k };
            Strength = k;
            PtA = P;
            xFix = X;
            yFix = Y;
            zFix = Z;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {            
            Vector3d V = PtA - p[PIndex[0]].Position;
            if (!xFix) { V.X = 0; }
            if (!yFix) { V.Y = 0; }
            if (!zFix) { V.Z = 0; }
            Move[0] = V;
            Weighting[0] = Strength;
        }
    }
}
