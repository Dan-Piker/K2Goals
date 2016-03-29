using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class PlasticAnchor : GoalObject
    {
        public Point3d Location;
        public double Limit;

        public PlasticAnchor(Point3d P, double R, double k)
        {
            PPos = new Point3d[1] { P };
            Move = new Vector3d[1];
            Weighting = new double[1] { k };
            Location = P;
            Limit = R;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Point3d ThisPt = p[PIndex[0]].Position;
            Vector3d Between = Location - ThisPt;
            Move[0] = Between;
            double stretch = Between.Length - Limit;
            if (stretch > 0)
            {
                Between.Unitize();
                Between *= stretch;
                Location -= Between;
                Move[0] -= Between;
            }
        }
    }
}
