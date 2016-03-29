using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Keep the angle between 2 line segments within a given range
    /// </summary>
    public class ClampAngle : GoalObject
    {
        public double EI;
        public double Upper;
        public double Lower;
   
        public ClampAngle(Line L0, Line L1, double Upp, double Low, double Strength)
        {
            PPos = new Point3d[4] { L0.From, L0.To, L1.From, L1.To };
            Move = new Vector3d[4];
            Weighting = new double[4];
            EI = Strength;
            Upper = Upp;
            Lower = Low;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Point3d P0 = p[PIndex[0]].Position;
            Point3d P1 = p[PIndex[1]].Position;
            Point3d P2 = p[PIndex[2]].Position;
            Point3d P3 = p[PIndex[3]].Position;

            Vector3d V01 = P1 - P0;
            Vector3d V23 = P3 - P2;

            double CurrentAngle = Vector3d.VectorAngle(V01, V23);
            double RestAngle = 0;
            bool Active = false;
            if (CurrentAngle > Upper)
            { 
                RestAngle = Upper; 
                Active = true; 
            }
            else if (CurrentAngle < Lower) 
            { 
                RestAngle = Lower; 
                Active = true; 
            }
           
            if(Active)
            {
                double top = Math.Sin(Vector3d.VectorAngle(V01, V23) - RestAngle);
                double Lc = (V01 + V23).Length;
                double Sa = top / (V01.Length * Lc);
                double Sb = top / (V23.Length * Lc);

                Vector3d Perp = Vector3d.CrossProduct(V01, V23);
                Vector3d ShearA = Vector3d.CrossProduct(V01, Perp);
                Vector3d ShearB = Vector3d.CrossProduct(Perp, V23);

                ShearA *= Sa * 0.5;
                ShearB *= Sb * 0.5;

                Move[0] = ShearA;
                Move[1] = -ShearA;
                Move[2] = ShearB;
                Move[3] = -ShearB;

                Weighting[0] = EI;
                Weighting[1] = EI;
                Weighting[2] = EI;
                Weighting[3] = EI;
            }
            else
            {
                Move[0] = Move[1] = Move[2] = Move[3] = Vector3d.Zero;
                Weighting[0] = Weighting[1] = Weighting[2] = Weighting[3] = 0;
            }           
        }     
    }
}
