using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Prevent closed polygons from intersecting by pushing them apart
    /// </summary>
    public class Collide2d : GoalObject
    {
        public double Strength;
        public Plane Pln;
        public List<List<int>> PolyIdx = new List<List<int>>();

        public Collide2d(List<Polyline> PL, Plane PLN, double k)
        {
            var Pts = new List<Point3d>();
            foreach (Polyline P in PL)
            {
                var C = P.ToNurbsCurve();
                var Ori = (int)C.ClosedCurveOrientation(PLN);
                if (Ori == -1) { P.Reverse(); }
                int Pre = Pts.Count;
                Pts.AddRange(P);
                var ThisPolyIdx = new List<int>();
                for (int i = 0; i < P.Count; i++)
                {
                    ThisPolyIdx.Add(Pre + i);
                }
                PolyIdx.Add(ThisPolyIdx);
            }

            PPos = Pts.ToArray();
            Weighting = new Double[Pts.Count];
            Move = new Vector3d[Pts.Count];
            for (int i = 0; i < Pts.Count; i++)
            {
                Weighting[i] = k;
            }
            Strength = k;
            Pln = PLN;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            var Crvs = new List<Curve>();
            for (int i = 0; i < PolyIdx.Count; i++)
            {
                Polyline NewPoly = new Polyline();
                for (int j = 0; j < PolyIdx[i].Count; j++)
                {
                    NewPoly.Add(p[PIndex[PolyIdx[i][j]]].Position);
                }

                Crvs.Add(NewPoly.ToNurbsCurve());
            }

            for (int i = 0; i < Move.Length; i++)
            {
                Move[i] = Vector3d.Zero;
            }
            for (int i = 0; i < Crvs.Count - 1; i++)
            {
                for (int j = i + 1; j < Crvs.Count; j++)
                {
                    if (Curve.PlanarCurveCollision(Crvs[i], Crvs[j], Pln, 0.0001))
                    {
                        Curve CI = Curve.ProjectToPlane(Crvs[i], Pln);
                        Curve CJ = Curve.ProjectToPlane(Crvs[j], Pln);

                        var Crossings = Rhino.Geometry.Intersect.Intersection.CurveCurve(CI, CJ, 0.0001, 0.0001);

                        if (Crossings.Count > 1)
                        {
                            Point3d P0 = Crossings[0].PointA;
                            Point3d P1 = Crossings[1].PointA;

                            var XA = new Point3d();
                            var XB = new Point3d();

                            double Max = CI.Domain.T1;
                            double t = 0;
                            if ((Crossings[1].ParameterA - Crossings[0].ParameterA) < 0.5 * Max)
                            {
                                t = 0.5 * (Crossings[1].ParameterA + Crossings[0].ParameterA);
                            }
                            else
                            {
                                double diff = Max + Crossings[0].ParameterA - Crossings[1].ParameterA;
                                t = (Crossings[1].ParameterA + 0.5 * diff) % Max;
                            }
                            XA = CI.PointAt(t);

                            Max = CJ.Domain.T1;
                            if ((Crossings[1].ParameterB - Crossings[0].ParameterB) < 0.5 * Max)
                            {
                                t = 0.5 * (Crossings[1].ParameterB + Crossings[0].ParameterB);
                            }
                            else
                            {
                                double diff = Max + Crossings[0].ParameterB - Crossings[1].ParameterB;
                                t = (Crossings[1].ParameterB + 0.5 * diff) % Max;
                            }
                            XB = CJ.PointAt(t);

                            Vector3d Push = 0.5 * (XA - XB);
                            var Poly = new Polyline();
                            CJ.TryGetPolyline(out Poly);
                            Move[PolyIdx[j][Poly.ClosestIndex(P1)]] = Push;
                            CI.TryGetPolyline(out Poly);
                            Move[PolyIdx[i][Poly.ClosestIndex(P0)]] = -Push; ;
                        }
                    }
                }
            }
        }
    }
}
