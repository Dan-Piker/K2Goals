using System.Collections.Generic;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Keep the length of a line between some upper and lower bounds
    /// When it is between these lengths no force is applied
    /// </summary>
    public class ClampLength : GoalObject
    {
        public double Upper;
        public double Lower;
        public double Stiffness;

        public ClampLength()
        {
        }

        public ClampLength(int S, int E, double U, double L, double k)
        {
            PIndex = new int[2] { S, E };
            Move = new Vector3d[2];
            Weighting = new double[2];
            Upper = U;
            Lower = L;
            Stiffness = k;
        }

        public ClampLength(Point3d S, Point3d E, double U, double L, double k)
        {
            PPos = new Point3d[2] { S, E };
            Move = new Vector3d[2];
            Weighting = new double[2]{k,k};
            Upper = U;
            Lower = L;
            Stiffness = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Vector3d current = p[PIndex[1]].Position - p[PIndex[0]].Position;
            double LengthNow = current.Length;
            if (LengthNow > Upper)
            {
                double stretchfactor = 1.0 - Upper / LengthNow;
                Vector3d SpringMove = 0.5 * current * stretchfactor;
                Move[0] = SpringMove;
                Move[1] = -SpringMove;
                Weighting[0] = Stiffness;
                Weighting[1] = Stiffness;
            }
            else if (LengthNow < Lower)
            {
                double stretchfactor = 1.0 - Lower / LengthNow;
                Vector3d SpringMove = 0.5 * current * stretchfactor;
                Move[0] = SpringMove;
                Move[1] = -SpringMove;
                Weighting[0] = Stiffness;
                Weighting[1] = Stiffness;
            }
            else
            {
                Move[0] = Vector3d.Zero;
                Move[1] = Vector3d.Zero;
            }
        }   
    }
}
