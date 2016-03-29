using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Set the volume of a mesh
    /// </summary>
    public class Volume : GoalObject
    {
        private Mesh M;
        public double TargetVol;
        public double Strength;

        public Volume(Mesh m, double v, double k)
        {
            PPos = m.Vertices.ToPoint3dArray();
            int L = m.Vertices.Count;
            Move = new Vector3d[L];
            Weighting = new double[L];
            for (int i = 0; i < L; i++)
            {
                Weighting[i] = k;
            }
            M = m;
            TargetVol = v;
            Strength = k;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            double TotalArea = 0;
            double TotalVolume = 0;
            Vector3d[] Cross = new Vector3d[M.Faces.Count];
            var FaceArea = new double[M.Faces.Count];

            for (int i = 0; i < M.Faces.Count; i++)
            {
                var P0 = p[PIndex[M.Faces[i].A]].Position;
                var P1 = p[PIndex[M.Faces[i].B]].Position;
                var P2 = p[PIndex[M.Faces[i].C]].Position;
                Cross[i] = Vector3d.CrossProduct(P1 - P0, P2 - P0);
                double Area = 0.5 * Cross[i].Length;
                FaceArea[i] = Area;
                TotalArea += Area;
                TotalVolume += Cross[i] * ((Vector3d)(P0));
            }

            TotalVolume /= 6.0;

            double VolumeShortage = TargetVol - TotalVolume;
            double Offset = VolumeShortage / TotalArea;

            for (int i = 0; i < M.Vertices.Count; i++)
            {Move[i] = Vector3d.Zero;}

            var VFaceWeights = new double[M.Vertices.Count];

            for (int i = 0; i < M.Faces.Count; i++)
            {
                Vector3d PushOut = Offset * Cross[i];
    
                Move[M.Faces[i].A] += PushOut;
                Move[M.Faces[i].B] += PushOut;
                Move[M.Faces[i].C] += PushOut;
                VFaceWeights[M.Faces[i].A] += FaceArea[i];
                VFaceWeights[M.Faces[i].B] += FaceArea[i];
                VFaceWeights[M.Faces[i].C] += FaceArea[i];
            }

            for (int i = 0; i < M.Vertices.Count; i++)
            {
       
                Move[i] /= VFaceWeights[i];
                Weighting[i] = Strength * VFaceWeights[i];
            }
        }

    }
}
