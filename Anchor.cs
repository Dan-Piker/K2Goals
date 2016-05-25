using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Fix a point in place
    /// </summary>   
    public class Anchor : GoalObject
    {
        public double Strength;
        public Point3d Pt;

        public Anchor()
        {
        }

        /// <summary>
        /// Construct a new Anchor object by particle index and target position
        /// </summary>
        /// <param name="Id">The integer index of the particle to anchor.</param>
        /// <param name="P">The target position to keep the particle at.</param>
        /// <param name="K">Strength of the Anchor. For an absolute anchor, you can use double.MaxValue here.</param>
        public Anchor(int Id, Point3d P, double k)
        {
            PIndex = new int[1] { Id };
            Move = new Vector3d[1];
            Weighting = new double[1] { k };
            Strength = k;
            Pt = P;
        }

        /// <summary>
        /// Construct a new Anchor object by position.
        /// </summary>        
        /// <param name="P">Particle starting position. Also used as the target position to keep the particle at.</param>
        /// <param name="K">Strength of the Anchor. For an absolute anchor, you can use double.MaxValue here.</param>
        public Anchor(Point3d P, double k)
        {
            PPos = new Point3d[1] { P };
            Move = new Vector3d[1];
            Weighting = new double[1]{k};
            Strength = k;
            Pt = P;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Move[0] = Pt - p[PIndex[0]].Position;
            Weighting[0] = Strength;
        }      
    }
}
