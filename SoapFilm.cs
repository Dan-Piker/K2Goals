using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Simple isotropic soap film element, as used for finding minimal surfaces
    /// Equivalent to cotan weighted Laplacian smoothing
    /// http://people.bath.ac.uk/abscjkw/LectureNotes/LightweightStructures/OtherMaterial/SoapFilmElement.pdf
    /// http://www.cs.cmu.edu/~kmcrane/Projects/DGPDEC/paper.pdf
    /// </summary>
    public class SoapFilm : GoalObject
    {
        public double Strength;

        public SoapFilm(Point3d[] Pts, double k)
        {
            PPos = Pts;
            Move = new Vector3d[3];
            Weighting = new double[3] { k, k, k };
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Point3d PA = p[PIndex[0]].Position;
            Point3d PB = p[PIndex[1]].Position;
            Point3d PC = p[PIndex[2]].Position;

            Vector3d AB = PB - PA;
            Vector3d BC = PC - PB;
            Vector3d CA = PA - PC;

            Vector3d Normal = Vector3d.CrossProduct(AB, BC);
            Normal.Unitize();

            Vector3d V0 = 0.5 * Vector3d.CrossProduct(BC, Normal);
            Vector3d V1 = 0.5 * Vector3d.CrossProduct(CA, Normal);
            Move[0] = V0;
            Move[1] = V1;
            Move[2] = -V0 - V1;

            Weighting[0] = Strength;
            Weighting[1] = Strength;
            Weighting[2] = Strength;
        }
    }
}
