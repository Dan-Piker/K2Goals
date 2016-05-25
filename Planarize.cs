using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class Planarize : GoalObject
    {
        public double Strength;
        private Vector3d FlatV;

        public Planarize()
        {
        }

        public Planarize(double k, int[] P)
        {
            int L = P.Length;
            PIndex = P;
            Move = new Vector3d[L];
            Weighting = new double[L];
            Strength = k;
        }

        public Planarize(double k, Point3d[] P)
        {
            int L = P.Length;
            PPos = P;
            Move = new Vector3d[L];
            Weighting = new double[L];
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            int L = PIndex.Length;
            Array.Clear(Move, 0, L);
            Array.Clear(Weighting, 0, L);

            if (L == 4)
            { PlanarizeQuad(p, 0, 1, 2, 3); }

            if (L > 4)
            {
                for (int i = 0; i < L; i++)
                { PlanarizeQuad(p, i, (i + 1) % L, (i + 2) % L, (i + 3) % L); }
            }           
        }

        public void PlanarizeQuad(List<KangarooSolver.Particle> p, int P0, int P1, int P2, int P3)
        {
            Line LA = new Line(p[PIndex[P0]].Position, p[PIndex[P2]].Position);
            Line LB = new Line(p[PIndex[P1]].Position, p[PIndex[P3]].Position);
            double tA, tB;
            Rhino.Geometry.Intersect.Intersection.LineLine(LA, LB, out tA, out tB);            
            Vector3d Flatten = LB.PointAt(tB) - LA.PointAt(tA);
            FlatV = Flatten;

            Flatten = Flatten * 0.5;
            Move[P0] += Flatten;
            Move[P2] += Flatten;
            Move[P1] -= Flatten;
            Move[P3] -= Flatten;

            Weighting[P0] = Strength;
            Weighting[P1] = Strength;
            Weighting[P2] = Strength;
            Weighting[P3] = Strength;
        }
   
        public override object Output(List<Particle> p)
        {
            return FlatV.Length;
        }
    }
}
