using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class SolidPoint : GoalObject
    {
        public double Strength;
        public Mesh _m;
        public bool Interior;

        public SolidPoint(List<Point3d> V, Mesh M,bool interior, double k)
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
            Interior = interior;
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            for (int i = 0; i < PIndex.Length; i++)
            {
                Point3d ThisPt = p[PIndex[i]].Position;
                bool Inside = _m.IsPointInside(ThisPt, 0.000001, true);
                if(Inside != Interior)
                {
                    Move[i] = _m.ClosestPoint(ThisPt) - ThisPt;
                    Weighting[i] = Strength;
                }
                else
                {
                    Move[i] = Vector3d.Zero;
                    Weighting[i] = 0;
                }
                
            }
        }      
    }
}
