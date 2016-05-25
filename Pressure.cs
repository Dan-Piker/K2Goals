using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class Pressure : GoalObject
    {
        public Pressure(Point3d[] Pts, double k)
        {
            PPos = Pts;
            Move = new Vector3d[3];
            Weighting = new double[3] { k, k, k };
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Point3d PA = p[PIndex[0]].Position;
            Point3d PB = p[PIndex[1]].Position;
            Point3d PC = p[PIndex[2]].Position;

            Vector3d AB = PB - PA;
            Vector3d BC = PC - PB;
            Vector3d CA = PA - PC;

            Vector3d Normal = Vector3d.CrossProduct(AB, BC); //this gives us a vector normal to the triangle, and of length twice its area            
            //halve this and divide it evenly over the 3 vertices. TODO - add Cotan weighting option here
            Vector3d PressureVector = (1.0 / 6.0) * Normal;

            Move[0] = Move[1] = Move[2] = PressureVector;
        }
    }
}
