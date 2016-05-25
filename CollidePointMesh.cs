using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// 2 way collision between points and mesh
    /// </summary>
    public class CollidePointMesh : GoalObject
    {
        public double Strength;
        public bool Inside;
        public Mesh M;

        public CollidePointMesh(Point3d[] P, Mesh m, bool inside, double K)
        {
            var mPoints = m.Vertices.ToPoint3dArray();

            var size = P.Length + mPoints.Length;
            PPos = new Point3d[size];

            Move = new Vector3d[size];
            Weighting = new double[size];

            for (int i = 0; i < mPoints.Length; i++)
            {
                PPos[i] = mPoints[i];
                Weighting[i] = K;
            }

            for (int i = mPoints.Length; i < PPos.Length; i++)
            {
                PPos[i] = P[i - mPoints.Length];
                Weighting[i] = K;
            }
            M = m;
            Inside = inside;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            for (int i = 0; i < M.Vertices.Count; i++)
            {
                M.Vertices.SetVertex(i, p[PIndex[i]].Position);
                Move[i] = Vector3d.Zero;
            }

            for (int i = M.Vertices.Count; i < PIndex.Length; i++)
            {
                Point3d ThisPt = p[PIndex[i]].Position;
                if (M.IsPointInside(ThisPt, 0.01, true)!=Inside)
                {
                    var MP = M.ClosestMeshPoint(ThisPt, 1000);
                    var Push = 0.5 * (MP.Point - ThisPt);
                    Move[i] = Push;

                    Move[M.Faces[MP.FaceIndex].A] -= Push * MP.T[0];
                    Move[M.Faces[MP.FaceIndex].B] -= Push * MP.T[1];
                    Move[M.Faces[MP.FaceIndex].C] -= Push * MP.T[2];
                }
                else { Move[i] = Vector3d.Zero; }
            }


        }
    }
}
