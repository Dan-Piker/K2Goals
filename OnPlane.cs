using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class OnPlane : GoalObject
    {
        public double Strength;
        public Plane _plane;

        public OnPlane()
        {
        }

        public OnPlane(List<int> V, Plane Pl, double k)
        {
            int L = V.Count;
            PIndex = V.ToArray();
            Move = new Vector3d[L];
            Weighting = new double[L];
            for (int i = 0; i < L; i++)
            {
                Weighting[i] = k;
            }
            _plane = Pl;
            Strength = k;
        }

        public OnPlane(List<Point3d> V, Plane Pl, double k)
        {
            int L = V.Count;
            PPos = V.ToArray();
            Move = new Vector3d[L];
            Weighting = new double[L];
            for (int i = 0; i < L; i++)
            {
                Weighting[i] = k;
            }
            _plane = Pl;
            Strength = k;
        }
       
        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            for (int i = 0; i < PIndex.Length; i++)
            {
                Point3d ThisPt = p[PIndex[i]].Position;
                Move[i] = _plane.ClosestPoint(ThisPt) - ThisPt;
                Weighting[i] = Strength;
            }           
        }
        
    }
}
