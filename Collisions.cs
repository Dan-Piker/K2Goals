using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class Collider : GoalObject
    {
        public double Strength;
        public List<double> Radii;
        public List<int> ObjectType;
        public List<List<int>> ObjectIndices;
        public List<int> IgnoreA;
        public List<int> IgnoreB;

        public Collider()
        { }
        public Collider(List<Object> Objects, List<double> Radii, List<int> PairsToIgnoreA, List<int> PairsToIgnoreB, double Strength)
        {
            this.Strength = Strength;
            this.Radii = Radii;
            ObjectType = new List<int>();
            ObjectIndices = new List<List<int>>();

            IgnoreA = new List<int>();
            IgnoreB = new List<int>();
            //make sure that A is always smaller than B
            for (int i = 0; i < PairsToIgnoreA.Count; i++)
            {
                if (PairsToIgnoreA[i] < PairsToIgnoreB[i])
                {
                    IgnoreA.Add(PairsToIgnoreA[i]);
                    IgnoreB.Add(PairsToIgnoreB[i]);
                }
                else
                {
                    IgnoreA.Add(PairsToIgnoreB[i]);
                    IgnoreB.Add(PairsToIgnoreA[i]);
                }
            }

            var PPosList = new List<Point3d>();
            int IndexCounter = 0;

            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i] is Point3d)
                {
                    ObjectType.Add(0);
                    PPosList.Add((Point3d)(Objects[i]));
                    ObjectIndices.Add(new List<int> { IndexCounter });
                    IndexCounter++;
                }
                else if (Objects[i] is Line)
                {
                    ObjectType.Add(1);
                    var L = (Line)(Objects[i]);
                    PPosList.Add(L.From);
                    PPosList.Add(L.To);
                    ObjectIndices.Add(new List<int> { IndexCounter, IndexCounter + 1 });
                    IndexCounter += 2;
                }
            }

            PPos = PPosList.ToArray();
            Move = new Vector3d[PPos.Length];
            Weighting = new double[PPos.Length];
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {

            // get all the boundingbox collisions
            // then calculate the actual collision response

            for (int i = 0; i < PPos.Length; i++)
            {
                Move[i] = Vector3d.Zero;
                Weighting[i] = 0;
            }

            var BoundingBoxes = new List<AABB>();

            for (int i = 0; i < Radii.Count; i++)
            {
                var Rad = Radii[i];

                if (ObjectType[i] == 0) // it is a ball
                {
                    var Pt = p[PIndex[ObjectIndices[i][0]]].Position;
                    var Low = new Point3d(Pt.X - Rad, Pt.Y - Rad, Pt.Z - Rad);
                    var High = new Point3d(Pt.X + Rad, Pt.Y + Rad, Pt.Z + Rad);
                    BoundingBoxes.Add(new AABB(Low, High, i));
                }
                else if (ObjectType[i] == 1) // it is a line
                {
                    var PtA = p[PIndex[ObjectIndices[i][0]]].Position;
                    var PtB = p[PIndex[ObjectIndices[i][1]]].Position;
                    var Low = new Point3d(
                      Math.Min(PtA.X, PtB.X) - Rad,
                      Math.Min(PtA.Y, PtB.Y) - Rad,
                      Math.Min(PtA.Z, PtB.Z) - Rad
                      );
                    var High = new Point3d(
                      Math.Max(PtA.X, PtB.X) + Rad,
                      Math.Max(PtA.Y, PtB.Y) + Rad,
                      Math.Max(PtA.Z, PtB.Z) + Rad
                      );
                    BoundingBoxes.Add(new AABB(Low, High, i));
                }
            }


            var PotentialCollisions = SAPCollide(BoundingBoxes);

            for (int i = 0; i < PotentialCollisions.Count; i++)
            {
                int Ix0 = PotentialCollisions[i].Item1;
                int Ix1 = PotentialCollisions[i].Item2;

                //first check if this is one of the collisions to ignore
                bool IgnoreThisPair = false;
                
                int foundInA = IgnoreA.IndexOf(Ix0);
                if (foundInA != -1)
                {
                    if (IgnoreB[foundInA] == Ix1)
                    {
                        IgnoreThisPair = true;
                    }

                    bool checkedAll = false;

                    while(checkedAll==false)
                    {
                        if (foundInA == IgnoreA.Count - 1)
                        {
                            checkedAll = true;
                        }
                        else 
                        {
                            foundInA = IgnoreA.IndexOf(Ix0, foundInA + 1);
                            if (foundInA == -1) 
                            { checkedAll = true; }
                            else 
                            {
                                if (IgnoreB[foundInA] == Ix1)
                                {
                                    IgnoreThisPair = true;
                                    checkedAll = true;
                                }                            
                            }                            
                        }
                    }                   
                }
                //I'm guessing there is a nicer way to do the above? 


                if (!IgnoreThisPair)
                {
                    //Narrow-phase collision check for different pair types

                    //Both balls:
                    if (ObjectType[Ix0] == 0 && ObjectType[Ix1] == 0)
                    {
                        int PIx0 = ObjectIndices[Ix0][0];
                        int PIx1 = ObjectIndices[Ix1][0];
                        var Pt0 = p[PIndex[PIx0]].Position;
                        var Pt1 = p[PIndex[PIx1]].Position;

                        Vector3d Separation = Pt1 - Pt0;
                        double LengthNow = Separation.Length;
                        double RadSum = Radii[Ix0] + Radii[Ix1];
                        if (LengthNow < RadSum)
                        {
                            double stretchfactor = 1.0 - RadSum / LengthNow;
                            Vector3d SpringMove = 0.5 * Separation * stretchfactor;
                            Move[PIx0] += SpringMove;
                            Move[PIx1] -= SpringMove;
                            Weighting[PIx0] = Strength;
                            Weighting[PIx1] = Strength;
                        }
                    }

                    //Ball-line
                    if ((ObjectType[Ix0] == 0 && ObjectType[Ix1] == 1) || (ObjectType[Ix0] == 1 && ObjectType[Ix1] == 0))
                    {
                        int LineStartIndex;
                        int LineEndIndex;
                        int BallCentreIndex;

                        if (ObjectType[Ix0] == 0)
                        {
                            BallCentreIndex = ObjectIndices[Ix0][0];
                            LineStartIndex = ObjectIndices[Ix1][0];
                            LineEndIndex = ObjectIndices[Ix1][1];
                        }
                        else
                        {
                            LineStartIndex = ObjectIndices[Ix0][0];
                            LineEndIndex = ObjectIndices[Ix0][1];
                            BallCentreIndex = ObjectIndices[Ix1][0];
                        }

                        Point3d LS = p[PIndex[LineStartIndex]].Position;
                        Point3d LE = p[PIndex[LineEndIndex]].Position;
                        Point3d P = p[PIndex[BallCentreIndex]].Position;

                        Vector3d V0 = LE - LS; //the vector of the line segment
                        Vector3d V1 = P - LS; //the vector from the start of the line to the sphere centre

                        double Length = V0.Length;
                        V0.Unitize();
                        double t = V0 * V1;  //project to get the parameter along the (infinite)line of the closest point
                        t = Math.Min(Math.Max(t, 0), Length); //take the point on just the segment

                        Vector3d Separation = V1 - V0 * t;
                        double SeparationLength = Separation.Length;
                        Separation.Unitize();

                        double R = Radii[Ix0] + Radii[Ix1];

                        if (SeparationLength < R)
                        {
                            Vector3d Push = 0.5 * Separation * (R - SeparationLength);
                            t = t / Length; //reparametrize t to lie between 0 and 1

                            Move[LineStartIndex] += (1 - t) * -Push;
                            Move[LineEndIndex] += t * -Push;
                            Move[BallCentreIndex] += Push;
                            Weighting[LineStartIndex] = Weighting[LineEndIndex] = Weighting[BallCentreIndex] = Strength;
                        }
                    }

                    //both lines:
                    if (ObjectType[Ix0] == 1 && ObjectType[Ix1] == 1)
                    {
                        int LAStartIndex = ObjectIndices[Ix0][0];
                        int LAEndIndex = ObjectIndices[Ix0][1];
                        int LBStartIndex = ObjectIndices[Ix1][0];
                        int LBEndIndex = ObjectIndices[Ix1][1];

                        var L1 = new Line(p[PIndex[LAStartIndex]].Position, p[PIndex[LAEndIndex]].Position);
                        var L2 = new Line(p[PIndex[LBStartIndex]].Position, p[PIndex[LBEndIndex]].Position);

                        Vector3d u = L1.To - L1.From;
                        Vector3d v = L2.To - L2.From;
                        Vector3d w = L1.From - L2.From;
                        double a = u * u;
                        double b = u * v;
                        double c = v * v;
                        double d = u * w;
                        double e = v * w;
                        double D = a * c - b * b;
                        double sc, sN, sD = D;
                        double tc, tN, tD = D;

                        // compute the line parameters of the two closest points
                        if (D < 1e-8)
                        { // the lines are almost parallel
                            sN = 0.0;         // force using point P0 on segment S1
                            sD = 1.0;         // to prevent possible division by 0.0 later
                            tN = e;
                            tD = c;
                        }
                        else
                        {
                            sN = b * e - c * d;
                            tN = a * e - b * d;

                            if (sN < 0.0)
                            {        // sc < 0 => the s=0 edge is visible
                                sN = 0.0;
                                tN = e;
                                tD = c;
                            }
                            else if (sN > sD)
                            {  // sc > 1  => the s=1 edge is visible
                                sN = sD;
                                tN = e + b;
                                tD = c;
                            }
                        }

                        if (tN < 0.0)
                        {            // tc < 0 => the t=0 edge is visible
                            tN = 0.0;
                            // recompute sc for this edge
                            if (-d < 0.0)
                                sN = 0.0;
                            else if (-d > a)
                                sN = sD;
                            else
                            {
                                sN = -d;
                                sD = a;
                            }
                        }
                        else if (tN > tD)
                        {      // tc > 1  => the t=1 edge is visible
                            tN = tD;
                            // recompute sc for this edge
                            if ((-d + b) < 0.0)
                                sN = 0;
                            else if ((-d + b) > a)
                                sN = sD;
                            else
                            {
                                sN = (-d + b);
                                sD = a;
                            }
                        }

                        // finally do the division to get sc and tc
                        sc = (Math.Abs(sN) < 1e-8 ? 0.0 : sN / sD);
                        tc = (Math.Abs(tN) < 1e-8 ? 0.0 : tN / tD);

                        // get the difference of the two closest points
                        Vector3d dP = w + (sc * u) - (tc * v);  // =  S1(sc) - S2(tc)


                        var Separation = -dP;
                        var Overlap = Radii[Ix0] + Radii[Ix1] - Separation.Length;
                        if (Overlap > 0)
                        {
                            Separation.Unitize();
                            var Push = 1 * Separation * Overlap;

                            Move[LAStartIndex] += (1 - sc) * -Push;
                            Move[LAEndIndex] += (sc) * -Push;

                            Move[LBStartIndex] += (1 - tc) * Push;
                            Move[LBEndIndex] += (tc) * Push;

                            Weighting[LAStartIndex] = Weighting[LAEndIndex] = Weighting[LBStartIndex] = Weighting[LBEndIndex] = Strength;
                        }
                    }
                }
            }
        }

        public static List<Tuple<int, int>> SAPCollide(List<AABB> boxes)
        {

            //this will send out the branch references for all the boxes that collide
            // List<int> CollideRef0 = new List<int>();
            // List<int> CollideRef1 = new List<int>();

            var CollideRefs = new List<Tuple<int, int>>();

            List<EndPoint> sortedInX = new List<EndPoint>();

            //add the endpoints in x
            foreach (AABB boxForPoint in boxes)
            {
                sortedInX.Add(boxForPoint.min[0]);//change this 0 to 1 or 2 to sort in Y or Z - will also need to change the second half of the test function
                sortedInX.Add(boxForPoint.max[0]);//change this 0 to 1 or 2 to sort in Y or Z
            }

            //sort by the num value of each EndPoint
            sortedInX.Sort(EndPoint.compareEndPoints);

            //this could be a list of intgers?
            List<int> openBoxes = new List<int>();

            foreach (EndPoint endPoint in sortedInX)
            {
                if (endPoint.isMin)
                {
                    AABB thisPointOwner = endPoint.owner;
                    //check against all in openBoxes
                    foreach (int openBoxRef in openBoxes)
                    {
                        //if it collides output the integers of the branches that collide
                        //do they collide in y?
                         if (thisPointOwner.max[1].num > boxes[openBoxRef].min[1].num && thisPointOwner.min[1].num < boxes[openBoxRef].max[1].num)
                        {
                            //they collide in y, do they collide in z?
                            if (thisPointOwner.max[2].num > boxes[openBoxRef].min[2].num && thisPointOwner.min[2].num < boxes[openBoxRef].max[2].num)
                            {
                                //they collide in z
                                //therefore they collide! Add to the list of collide refs
                                //make sure the lowest index comes first to make searching easier later
                                int CollideA = endPoint.owner.branchRef;
                                int CollideB = boxes[openBoxRef].branchRef;
                                CollideRefs.Add(new Tuple<int, int>(Math.Min(CollideA, CollideB), Math.Max(CollideA, CollideB)));
                            }
                        }
                    }
                    //add corresponding box to openBoxes
                    openBoxes.Add(thisPointOwner.branchRef);

                }
                else
                {
                    //it must be an max point
                    //remove corresponding box from openBoxes
                    openBoxes.Remove(endPoint.owner.branchRef);
                }
            }

            return CollideRefs;
        }


        public class AABB
        {
            public EndPoint[] min;//an array of size 3 with the x,y,z value for the AABB min
            public EndPoint[] max;//an array of size 3 with the x,y,z value for the AABB max
            public int branchRef;

            //constructor
            public AABB(Point3d tMin, Point3d tMax, int tBranchRef)
            {
                min = new EndPoint[] { new EndPoint(tMin.X, true, this), new EndPoint(tMin.Y, true, this), new EndPoint(tMin.Z, true, this) };
                max = new EndPoint[] { new EndPoint(tMax.X, false, this), new EndPoint(tMax.Y, false, this), new EndPoint(tMax.Z, false, this) };
                branchRef = tBranchRef;
            }
        }

        public class EndPoint
        {
            public AABB owner;
            public double num;//its actual value - corresponds to the x,y or z value
            public bool isMin;//to distinguish whether this is a min point or a max point

            //constructor
            public EndPoint(double tNum, bool tIsMin, AABB tOwner)
            {
                num = tNum;
                isMin = tIsMin;
                owner = tOwner;
            }

            //used as a comparison to sort the endpoints
            public static int compareEndPoints(EndPoint x, EndPoint y)
            {
                return x.num.CompareTo(y.num);
            }
        }

    }

}
