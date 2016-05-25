using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class DynamicWeight1d : GoalObject
    {
        public DynamicWeight1d(Point3d s, Point3d e, double WeightPerLength)
        {
            PPos = new Point3d[2] { s, e };
            Move = new Vector3d[2];
            Weighting = new double[2] { WeightPerLength, WeightPerLength };
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            double CurrentLength = p[PIndex[1]].Position.DistanceTo(p[PIndex[0]].Position);
            var Weight = new Vector3d(0, 0, CurrentLength);
            Move[0] = Move[1] = 0.5 * Weight;
        }
    }
}
