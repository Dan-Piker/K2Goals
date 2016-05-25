using System.Collections.Generic;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class Unary : GoalObject
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

        public override void Calculate(List<Particle> p)
        {            
            Move[0] = Force;                  
            Weighting[0] = 1.0;
        }
      
    }    
}
