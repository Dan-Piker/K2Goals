using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class TangentialSmooth : GoalObject
    {
        public double Strength;

        public TangentialSmooth(Point3d[] P, double k)
        {
            PPos = P;
            Move = new Vector3d[P.Length];
            Weighting = new double[P.Length];
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Point3d Avg = new Point3d();
            var Vecs = new Vector3d[PIndex.Length - 1];
            for (int i = 1; i < PIndex.Length; i++)
            {
                Avg = Avg + p[PIndex[i]].Position;
                Vecs[i - 1] = p[PIndex[i]].Position - p[PIndex[0]].Position;
            }
            double Inv = 1.0 / (PIndex.Length - 1);
            Avg = Avg * Inv;
            Vector3d Smooth = 0.5 * (Avg - p[PIndex[0]].Position);

            Vector3d Normal = new Vector3d();
            for (int i = 0; i < Vecs.Length; i++)
            {
                Normal += Vector3d.CrossProduct(Vecs[i], Vecs[(i + 1) % Vecs.Length]);
            }
            Normal.Unitize();
            Smooth -= Normal * (Normal * Smooth);

            Move[0] = Smooth;
            Weighting[0] = Strength;
            Smooth *= -Inv;
            for (int i = 1; i < PIndex.Length; i++)
            {
                Move[i] = Smooth;
                Weighting[i] = Strength;
            }
        }
    }
}
