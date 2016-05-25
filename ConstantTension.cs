using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class ConstantTension : GoalObject
    {
        public double Strength;

        public ConstantTension(Point3d s, Point3d e, double k)
        {
            PPos = new Point3d[2] { s, e };
            Move = new Vector3d[2];
            Weighting = new double[2];
            Strength = 2*k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Vector3d current = p[PIndex[1]].Position - p[PIndex[0]].Position;
            Move[0] = 0.5 * current;
            Move[1] = -0.5 * current;

            Weighting[0] = Weighting[1] = Strength / current.Length;
        }

        public override object Output(List<KangarooSolver.Particle> p)
        {
            return new Line(p[PIndex[0]].Position, p[PIndex[1]].Position);
        }
    }
}
