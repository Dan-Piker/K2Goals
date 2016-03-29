using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Collisions keeping points inside or outside a closed 2d curve in plane
    /// </summary>
    public class Curve2dPoint : GoalObject
    {
        public double Strength;
        public Plane Pln;        
        public Curve C;
        public bool Interior;

        public Curve2dPoint(List<Point3d> V, Curve Crv, Plane Pln,bool interior, double k)
        {
            int L = V.Count;
            PPos = V.ToArray();
            Move = new Vector3d[L];
            Weighting = new double[L];
            for (int i = 0; i < L; i++)
            {
                Weighting[i] = k;
            }
            C = Crv;
            this.Pln = Pln;
            Interior = interior;
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            for (int i = 0; i < PIndex.Length; i++)
            {
                Point3d ThisPt = p[PIndex[i]].Position;
                if (Interior != (C.Contains(ThisPt, Pln) == PointContainment.Inside))
                {
                    double t = new double();
                    C.ClosestPoint(ThisPt, out t);
                    Move[i] = C.PointAt(t) - ThisPt;
                    Weighting[i] = Strength;
                }
                else { Move[i] = Vector3d.Zero; Weighting[i] = 0; }
            }   
        }
    }
}
