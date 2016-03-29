using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{

    public class MagnetSnap : GoalObject
    {
        public double Strength;
        public double Range;
        public double RangeSq;

        public MagnetSnap(List<Point3d> V, double R, double k)
        {
            int L = V.Count;
            PPos = V.ToArray();
            Move = new Vector3d[L];
            Weighting = new double[L];
            for (int i = 0; i < L; i++)
            {
                Weighting[i] = k;
            }
            Range = R;
            RangeSq = R * R;
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            int L = PIndex.Length;
            double[] Xcoord = new double[L];
            for (int i = 0; i < L; i++)
            {
                Xcoord[i] = p[PIndex[i]].Position.X;
            }
            Array.Sort(Xcoord, PIndex);

            for (int i = 0; i < L; i++)
            {
                Move[i] = Vector3d.Zero;
                Weighting[i] = 0;
            }

            for (int i = 0; i < (PIndex.Length - 1); i++)
            {
                for (int j = 1; (i + j) < PIndex.Length; j++)
                {
                    int k = i + j;
                    Vector3d Separation = p[PIndex[k]].Position - p[PIndex[i]].Position;
                    if (Separation.X < Range)
                    {
                        if (Separation.SquareLength < RangeSq)
                        {
                            Move[i] += 0.5 * Separation;
                            Move[k] -= 0.5 * Separation;
                            Weighting[i] = Strength;
                            Weighting[k] = Strength;
                        }
                    }
                    else { break; }
                }
            }
        }

    }
}
