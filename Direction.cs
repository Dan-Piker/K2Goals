using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// This rotates a line segment to align it with a given vector direction.
    /// If no direction is supplied, it will take the closest of the +/- world XYZ directions, and can be used to snap geometry to orthogonal.
    /// </summary>
    public class Direction : GoalObject
    {
        public Vector3d Dir;
        public double Strength;

        public Direction()
        {
        }

        public Direction(int Start, int End, Vector3d Direction, double K)
        {
            PIndex = new int[2] { Start, End };
            Move = new Vector3d[2];
            Weighting = new double[2] { K, K };
            Dir = Direction;
            Dir.Unitize();
            Strength = K;
        }

        public Direction(Point3d Start, Point3d End, Vector3d Direction, double K)
        {
            PPos = new Point3d[2] { Start, End };
            Move = new Vector3d[2];
            Weighting = new double[2] { K, K };
            Dir = Direction;
            Dir.Unitize();
            Strength = K;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Point3d S = p[PIndex[0]].Position;
            Point3d E = p[PIndex[1]].Position;
            Vector3d V = E - S;
            Vector3d To = (V - (V * Dir) * Dir)*0.5;

            Move[0] = To;
            Move[1] = -To;

            Weighting[0] = Strength;
            Weighting[1] = Strength;
        }
  
    }
}
