using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class OnCurve : GoalObject
    {
        public double Strength;
        public Curve _curve;

        public OnCurve()
        {
        }

        public OnCurve(List<int> V, Curve C, double k)
        {
            int L = V.Count;
            PIndex = V.ToArray();
            Move = new Vector3d[L];
            Weighting = new double[L];
            for (int i = 0; i < L; i++)
            {
                Weighting[i] = k;
            }
            _curve = C;
            Strength = k;
        }

        public OnCurve(List<Point3d> V, Curve C, double k)
        {
            int L = V.Count;
            PPos = V.ToArray();
            Move = new Vector3d[L];
            Weighting = new double[L];
            for (int i = 0; i < L; i++)
            {
                Weighting[i] = k;
            }
            _curve = C;
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            for (int i = 0; i < PIndex.Length; i++)
            {
                Point3d ThisPt = p[PIndex[i]].Position;
                double t = new double();
                _curve.ClosestPoint(ThisPt, out t);
                Move[i] = _curve.PointAt(t) - ThisPt;                
                Weighting[i] = Strength;
            }
        }
      
    }
}
