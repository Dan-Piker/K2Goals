using System.Collections.Generic;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class Unary : IGoal
    {
        public Vector3d Force;        

        public Unary()
        {
        }

        public Unary(int u, Vector3d v)
        {
            PIndex = new int[1] { u };
            Move = new Vector3d[1];
            Weighting = new double[1];
            Force = v;
        }

        public Unary(Point3d P, Vector3d v)
        {
            PPos = new Point3d[1] { P };
            Move = new Vector3d[1];
            Weighting = new double[1];
            Force = v;
        }

        public Point3d[] PPos { get; set; }
        public int[] PIndex { get; set; }  
        public Vector3d[] Move { get; set; }
        public double[] Weighting { get; set; }
       
        public void Calculate(List<Particle> p)
        {            
            Move[0] = Force;                  
            Weighting[0] = 1.0;
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
