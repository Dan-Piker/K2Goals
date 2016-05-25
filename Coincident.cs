using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class Coincident : GoalObject
    {
        public double Strength;

        public Coincident(Point3d P0, Point3d P1, double Strength)
        {
            this.Strength = Strength;
            PPos = new Point3d[2] { P0, P1 };
            Move = new Vector3d[2];
            Weighting = new Double[2] { Strength, Strength };
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Move[0] = 0.5 * (p[PIndex[1]].Position - p[PIndex[0]].Position);
            Move[1] = -1 * Move[0];
        }
    }
}
