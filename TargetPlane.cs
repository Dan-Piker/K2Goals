using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Keep point on a given plane
    /// </summary>
    public class TargetPlane : GoalObject
    {
        public double Strength;
        public Plane _plane;

        public TargetPlane()
        {
        }

        public TargetPlane(int P, double k, Plane Target)
        {
            PIndex = new int[1] { P };
            Move = new Vector3d[1];
            Weighting = new double[1];
            Strength = k;
            _plane = Target;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Point3d ThisPt = p[PIndex[0]].Position;
            Move[0] = _plane.ClosestPoint(ThisPt) - ThisPt;
            Weighting[0] = Strength;
        }       
    }
}
