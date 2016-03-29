using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class FloorPlane : IGoal
    {
        public double Strength;

        public FloorPlane()
        {
        }

        public FloorPlane( double k)
        {
            PIndex = new int[1] {0};
            Move = new Vector3d[1];
            Weighting = new double[1];
            Strength = k;
        }

        public Point3d[] PPos { get; set; }
        public int[] PIndex { get; set; }
        public Vector3d[] Move { get; set; }
        public double[] Weighting { get; set; }

        public void Calculate(List<KangarooSolver.Particle> p)
        {           
            int L = p.Count;

            PIndex = new int[L];
            Move = new Vector3d[L];
            Weighting = new double[L];

            for (int i = 0; i < L; i++)
            {
                PIndex[i] = i;
                double Height = p[i].Position.Z;
                if (Height < 0)
                {
                    Move[i] = -Vector3d.ZAxis * Height;
                    Weighting[i] = Strength;
                }
                else
                {
                    Move[i] = Vector3d.Zero;
                    Weighting[i] = 0;
                }

            }
        }
        public IGoal Clone()
        {
            return this.MemberwiseClone() as IGoal;
        }
        public object Output(List<Particle> p)
        {
            return null;
        }
    }
}
