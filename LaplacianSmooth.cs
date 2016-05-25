using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Uniform Laplacian smoothing aka Umbrella operator    
    /// </summary>
    public class LaplacianSmooth : GoalObject
    {
        public double Strength;

        public LaplacianSmooth()
        {
        }

        public LaplacianSmooth(int[] P, double k)
        {            
            PIndex = P;                   
            Move = new Vector3d[P.Length];
            Weighting = new double[P.Length];
            Strength = k;
        }

        public LaplacianSmooth(Point3d[] P, double k)
        {
            PPos = P;
            Move = new Vector3d[P.Length];
            Weighting = new double[P.Length];
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Point3d Avg = new Point3d();
            for (int i = 1; i < PIndex.Length; i++)
            {
                Avg = Avg + p[PIndex[i]].Position;
            }
            double Inv = 1.0 / (PIndex.Length - 1);
            Avg = Avg * Inv;
            Vector3d Smooth = 0.5 * (Avg - p[PIndex[0]].Position);
            Move[0] = Smooth;
            Weighting[0] = Strength;
            Smooth *= -Inv;
            for (int i = 1; i < PIndex.Length; i++)
            {
                Move[i] = Smooth;
                Weighting[i] = Strength;
            }
        }
   
    }
}
