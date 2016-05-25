using System.Collections.Generic;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// For a pair of adjacent triangles in a mesh, make their incircles tangent    
    /// </summary>
    public class TangentIncircles : GoalObject
    {
        public double K;

        public TangentIncircles()
        {
        }

        public TangentIncircles(int P0, int P1, int P2, int P3, double Strength)
        {
            PIndex = new int[4] { P0, P1, P2, P3 }; 
            Move = new Vector3d[4];
            Weighting = new double[4];
            K = Strength;
        }

        public TangentIncircles(Point3d P0, Point3d P1, Point3d P2, Point3d P3, double Strength)
        {
            PPos = new Point3d[4] { P0, P1, P2, P3 }; 
            PIndex = new int[4];
            Move = new Vector3d[4];
            Weighting = new double[4];
            K = Strength;
        }
       
        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Point3d P0 = p[PIndex[0]].Position;
            Point3d P1 = p[PIndex[1]].Position; 
            Point3d P2 = p[PIndex[2]].Position; 
            Point3d P3 = p[PIndex[3]].Position;

            Vector3d V0 = P1 - P0;
            Vector3d V1 = P2 - P1;
            Vector3d V2 = P3 - P2;
            Vector3d V3 = P0 - P3;

            double L0 = V0.Length;
            double L1 = V1.Length;
            double L2 = V2.Length;
            double L3 = V3.Length;

            double L0L2 = L0 + L2;
            double L1L3 = L1 + L3;

            double MeanSum = 0.5 * (L0L2 + L1L3);

            double Stretch02 = 0.5 * (L0L2 - MeanSum);
            double Stretch13 = 0.5 * (L1L3 - MeanSum);

            V0.Unitize();
            V1.Unitize();
            V2.Unitize();
            V3.Unitize();

            Vector3d M0 = V0 * Stretch02;
            Vector3d M1 = V1 * Stretch13;
            Vector3d M2 = V2 * Stretch02;
            Vector3d M3 = V3 * Stretch13;

            Move[0] = M0 - M3;
            Move[1] = M1 - M0;
            Move[2] = M2 - M1;
            Move[3] = M3 - M2;

            Weighting[0] = Weighting[1] = Weighting[2] = Weighting[3] = K;     
        }      
    }
}