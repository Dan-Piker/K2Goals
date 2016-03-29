using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class PlasticHinge : GoalObject
    {
        public double RestAngle;
        public double PlasticLimit;

        public PlasticHinge(Point3d P0, Point3d P1, Point3d P2, Point3d P3, double Angle, double Plastic, double K)
        {
            PPos = new Point3d[4] { P0, P1, P2, P3 };
            Move = new Vector3d[4];
            Weighting = new double[4] { K, K, K, K };
            RestAngle = Angle;
            PlasticLimit = Plastic;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            Point3d P0 = p[PIndex[0]].Position;
            Point3d P1 = p[PIndex[1]].Position;
            Point3d P2 = p[PIndex[2]].Position;
            Point3d P3 = p[PIndex[3]].Position;

            Vector3d V01 = P1 - P0;
            Vector3d V02 = P2 - P0;
            Vector3d V03 = P3 - P0;
            Vector3d V21 = P1 - P2;
            Vector3d V31 = P1 - P3;

            double L01 = V01.Length;
            double invL01 = 1.0 / L01;

            double H0 = (V02 - V02 * invL01 * invL01 * V01 * V01).Length;
            double H1 = (V03 - V03 * invL01 * invL01 * V01 * V01).Length;
            double H = 0.5 / (H0 + H1);

            double Dot0201 = V02 * V01;
            double Dot0301 = V03 * V01;
            double Dot2101 = V21 * V01;
            double Dot3101 = V31 * V01;

            Vector3d Cross0 = Vector3d.CrossProduct(V02, V01);
            Vector3d Cross1 = Vector3d.CrossProduct(V01, V03);

            double CurrentAngle = Vector3d.VectorAngle(Cross0, Cross1, new Plane(P0, V01));
            if (CurrentAngle > Math.PI) { CurrentAngle = CurrentAngle - 2 * Math.PI; }

            double AngleError = CurrentAngle - RestAngle;

            double OverFold = Math.Abs(AngleError) - PlasticLimit; // the amount of folding beyond the elastic/plastic threshold
            if (OverFold > 0)
            {
                if (AngleError > 0)
                {
                    RestAngle += OverFold;
                    AngleError -= OverFold;
                }
                else
                {
                    RestAngle -= OverFold;
                    AngleError += OverFold;
                }
            }

            double InvL = 1.0 / Cross0.Length;
            double Cot0u = Dot0201 * InvL;
            double Cot0v = Dot2101 * InvL;
            InvL = 1.0 / Cross1.Length;
            double Cot1u = Dot0301 * InvL;
            double Cot1v = Dot3101 * InvL;

            double D = AngleError * H * 0.5;
            Vector3d A = Cross0 * D;
            Move[0] = Cot0v * A;
            Move[1] = Cot0u * A;
            Move[2] = -(Cot0v + Cot0u) * A;
            A = Cross1 * D;
            Move[0] += Cot1v * A;
            Move[1] += Cot1u * A;
            Move[3] = -(Cot1v + Cot1u) * A;
        }
    }
}
