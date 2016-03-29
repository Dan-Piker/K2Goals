using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class PlasticLength : GoalObject
    {
        public double RestLength;
        public double Limit;
        public double Stiffness;

        public PlasticLength(Point3d S, Point3d E, double Lim, double k)
        {
            PPos = new Point3d[2] { S, E };
            Move = new Vector3d[2];
            Weighting = new double[2];
            RestLength = S.DistanceTo(E);
            Limit = Lim;
            Stiffness = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Vector3d current = p[PIndex[1]].Position - p[PIndex[0]].Position;
            double CurrentLength = current.Length;
            double Stretch = CurrentLength - RestLength;

            if (Stretch > Limit)
            {
                RestLength += Stretch - Limit;
            }
            if (-Stretch > Limit)
            {
                RestLength += Stretch + Limit;
            }

            double stretchfactor = 1.0 - RestLength / CurrentLength;
            Vector3d SpringMove = 0.5 * current * stretchfactor;
            Move[0] = SpringMove;
            Move[1] = -SpringMove;

            Weighting[0] = 2 * Stiffness;
            Weighting[1] = 2 * Stiffness;
        }
    }
}
