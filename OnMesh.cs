using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class OnMesh : GoalObject
    {
        public double Strength;
        public Mesh _m;

        public OnMesh()
        {
        }

        public OnMesh(List<int> V, Mesh M, double k)
        {
            int L = V.Count;
            PIndex = V.ToArray();
            Move = new Vector3d[L];
            Weighting = new double[L];
            for (int i = 0; i < L; i++)
            {
                Weighting[i] = k;
            }
            _m = M;
            Strength = k;
        }

        public OnMesh(List<Point3d> V, Mesh M, double k)
        {
            int L = V.Count;
            PPos = V.ToArray();
            Move = new Vector3d[L];
            Weighting = new double[L];
            for (int i = 0; i < L; i++)
            {
                Weighting[i] = k;
            }
            _m = M;
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            for (int i = 0; i < PIndex.Length; i++)
            {
                Point3d ThisPt = p[PIndex[i]].Position;

                Move[i] = _m.ClosestPoint(ThisPt) - ThisPt;
                Weighting[i] = Strength;
            }
        }
      
    }
}
