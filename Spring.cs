using System.Collections.Generic;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class Spring : GoalObject
    {
        public double RestLength, Stiffness;

        public Spring()
        {
        }

        public Spring(int s, int e, double l, double k)
        {
            PIndex = new int[2] { s, e };
            Move = new Vector3d[2];
            Weighting = new double[2];
            RestLength = l;
            Stiffness = k;
        }

        public Spring(Point3d s, Point3d e, double l, double k)
        {          
            PPos = new Point3d[2] { s, e };
            Move = new Vector3d[2];
            Weighting = new double[2];
            RestLength = l;
            Stiffness = k;
        }
        
        public override void Calculate(List<Particle> p)
        {
            Vector3d current = p[PIndex[1]].Position - p[PIndex[0]].Position;
            double stretchfactor = 1.0 - RestLength / current.Length;
            Vector3d SpringMove = 0.5 * current * stretchfactor;
            Move[0] = SpringMove;
            Move[1] = -SpringMove;           
            Weighting[0] = 2*Stiffness;
            Weighting[1] = 2*Stiffness;
        }
       
        public override object Output(List<Particle> p)
        {
            return new Line(p[PIndex[0]].Position, p[PIndex[1]].Position);
        }
    }  
}
