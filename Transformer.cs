using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class Transformer : GoalObject
    {
        public double Strength;
        public Transform XForm;
        public Transform Inverse;

        public Transformer()
        {
        }

        public Transformer(int P0, int P1, Transform T, double k)
        {
            PIndex = new int[2] { P0, P1 };
            Move = new Vector3d[2];
            Weighting = new double[2] { k, k };
            Strength = k;
            XForm = T;
            T.TryGetInverse(out Inverse);
        }

        public Transformer(Point3d P0, Point3d P1, Transform T, double k)
        {
            PPos = new Point3d[2] { P0, P1 };
            Move = new Vector3d[2];
            Weighting = new double[2] { k, k };
            Strength = k;
            XForm = T;
            T.TryGetInverse(out Inverse);
        }
       
        public override void Calculate(List<KangarooSolver.Particle> p)
        {

            Point3d PT0 = p[PIndex[0]].Position;
            PT0.Transform(XForm);
            Vector3d Match = PT0 - p[PIndex[1]].Position;
            Move[1] = 0.5 * Match;

            Point3d PT1 = p[PIndex[1]].Position;
            PT1.Transform(Inverse);
            Match = PT1 - p[PIndex[0]].Position;

            Move[0] = 0.5 * Match;
            Weighting[0] = Weighting[1] = Strength;
        }

      
    }
}
