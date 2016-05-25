using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Make distance between 2 points a multiple of some given factor
    /// </summary>
    public class LengthMultiple : GoalObject
    {
        public double Stiffness;
        public double Factor;

        public LengthMultiple()
        {
        }

        public LengthMultiple(int S, int E, double F, double k)
        {
            PIndex = new int[2] { S, E };
            Move = new Vector3d[2];
            Weighting = new double[2];
            Stiffness = k;
            Factor = F;
        }

        public LengthMultiple(Point3d S, Point3d E, double F, double k)
        {
            PPos = new Point3d[2] { S, E };
            Move = new Vector3d[2];
            Weighting = new double[2];
            Stiffness = k;
            Factor = F;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Vector3d current = p[PIndex[1]].Position - p[PIndex[0]].Position;
            double LengthNow = current.Length;
            double RestLength = (Math.Round(LengthNow / Factor)) * Factor;
            double stretchfactor = 1.0 - RestLength / LengthNow;
            Vector3d SpringMove = 0.5 * current * stretchfactor;
            Move[0] = SpringMove;
            Move[1] = -SpringMove;
            Weighting[0] = 2*Stiffness;
            Weighting[1] = 2*Stiffness;
        } 

    }
}
