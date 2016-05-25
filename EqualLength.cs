using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Make a set of lines equal length
    /// </summary>
    public class EqualLength : GoalObject
    {
        public double Strength;

        public EqualLength()
        {
        }

        public EqualLength(int[] S, int[] E, double k)
        {
            int L = S.Length;
            PIndex = new int[L * 2];
            S.CopyTo(PIndex, 0);
            E.CopyTo(PIndex, L);
            Move = new Vector3d[L * 2];
            Weighting = new double[L * 2];
            Strength = k;
        }

        public EqualLength(List<Curve> Crvs, double k)
        {
            int L = Crvs.Count;            
            PPos = new Point3d[L * 2];
            for (int i = 0; i < L;i++ )
            {
                PPos[i] = Crvs[i].PointAtStart;
                PPos[i + L] = Crvs[i].PointAtEnd;
            }
            Move = new Vector3d[L * 2];
            Weighting = new double[L * 2];
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            int L = PIndex.Length / 2;
            double AvgLength = 0;

            for (int i = 0; i < L; i++)
            {
                AvgLength += p[PIndex[i]].Position.DistanceTo(p[PIndex[i + L]].Position);
            }
            AvgLength *= 1.0 / L;

            for (int i = 0; i < L; i++)
            {
                Vector3d current = p[PIndex[i + L]].Position - p[PIndex[i]].Position;
                double stretchfactor = 1.0 - AvgLength / current.Length;
                Vector3d SpringMove = 0.5 * current * stretchfactor;
                Move[i] = SpringMove;
                Move[i + L] = -SpringMove;
                Weighting[i] = Strength;
                Weighting[i + L] = Strength;
            }
        }
  
    }
}
