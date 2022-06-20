using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if NET_2_0

using UtyRx;

#else

using UniRx;

#endif

namespace myField.Utils
{
    internal static class Vector3Utils
    {
        public static Vector3 GetAvg<T>(IEnumerable<T> values, Func<T, float> ToValue)
        {
            if (values.Count() == 0)
            {
                return Vector3.zero;
            }
            else
            {
                var min = float.MaxValue;
                var max = -float.MaxValue;
                var avg = 0f;
                foreach (var item in values)
                {
                    var val = ToValue(item);
                    if (val < min) min = val;
                    if (val > max) max = val;
                    avg += val;
                }
                avg /= values.Count();

                return new Vector3(avg, min, max);
            }
        }

        public static float Distance2Line(this Vector3 pos, Vector3 a, Vector3 b)
        {
            var a2pos = new Vector3(pos.x - a.x, pos.y - a.y, pos.z - a.z);
            var a2b = new Vector3(b.x - a.x, b.y - a.y, b.z - a.z);
            var cross = Vector3.Cross(a2pos, a2b);
            var d = cross.magnitude / a2b.magnitude;
            return d;
        }

        public static float[] GetHeights(IList<Vector3> vertices)
        {
            var heights = new float[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
                heights[i] = vertices[i].y;
            return heights;
        }

        public static float GetAngleXZ(this Vector3 dir)
        {
            var angle = Mathf.Atan2(dir.z, dir.x);
            return angle * Mathf.Rad2Deg;
        }

        public static float GetHorizontalAngle(this Vector3 dir)
        {
            var angle = dir.GetAngleXZ();
            if (angle < 0)
            {
                angle += 360;
            }
            angle = ((360 - angle) + 90) % 360;
            return angle;
        }

        public static float GetVerticalAngle(this Vector3 dir)
        {
            var t = new Transformation(Vector3.zero, Quaternion.AngleAxis(-dir.GetHorizontalAngle(), Vector3.up));
            var n = t.Transform(dir);
            var angle = Mathf.Atan2(n.y, n.z);
            return (90 - angle * Mathf.Rad2Deg);
        }

        public static float DistanceToLineSegment(this Vector3 pos, Vector3 a, Vector3 b, out Vector3 closetPointOnLineSegment)
        {
            //https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
            var dot = Vector3.Dot((a - pos), (b - a));
            dot /= Vector3.Dot((b - a), (b - a));

            // test if the lot from pos onto to the line hits the segment between a and b
            //isBetween = (dot >= -1f && dot <= 0f);
            //if (!isBetween)
            //{
            //    // if not, clip dot so that the closestPointOnLineSegemnt is between a and b
            if (dot > 0) { dot = 0; }
            else if (dot < -1) { dot = -1; }
            //}

            closetPointOnLineSegment = a - dot * (b - a);
            var d = ((a - pos) - dot * (b - a)).magnitude;

            return d;
        }

        public static List<Vector3> Rotate(ICollection<Vector3> vertices, Quaternion rotation)
        {
            var newPos = new List<Vector3>(vertices.Count);
            foreach (var item in vertices)
                newPos.Add(rotation * item);
            return newPos;
        }

        public static List<Vector3> Transform(this IList<Vector3> pos, Quaternion q, Vector3 t = new Vector3())
        {
            var newPos = new List<Vector3>(pos.Count);
            for (int i = 0; i < pos.Count; i++)
                newPos.Add(pos[i].Transform(q, t));

            return newPos;
        }

        public static List<Vector3> Interpolate(IList<Vector3> path, bool isClosed = true, float internodeDistance = 1f)
        {
            var iPath = new List<Vector3>();
            if (path.Count > 0)
            {
                iPath.Add(path[0]);
                for (int i = 1; i < path.Count; i++)
                    iPath.AddRange(Interpolate(path[i - 1], path[i], internodeDistance));
                if (isClosed)
                    iPath.AddRange(Interpolate(path[path.Count - 1], path[0], internodeDistance));
            }

            return iPath;
        }

        public static List<Vector3> Interpolate(Vector3 v0, Vector3 v1, float interNodeDistance)
        {
            float d = Vector3.Distance(v0, v1);
            int nInter = (int)(d / interNodeDistance);

            Vector3 dir = (v1 - v0).normalized;
            dir *= d / (nInter + 1); // distributes vertices equally close to given interNodeDistance

            //dir *= interNodeDistance;
            //Debug.Log("dist = " + d + ", ninter = " + nInter + ", dir = " + dir.magnitude);

            //vertices.Add(v1); // note: i = 0 adds v1

            var vertices = new List<Vector3>();

            for (int i = 0; i < nInter + 1; i++)
            {
                var v = (v0 + dir * (i + 1));
                vertices.Add(v);
            }

            return vertices;
        }

        public static bool IntersectsXZ(Vector3 v0, Vector3 v1, Vector3 w0, Vector3 w1)
        {
            // x-z plane
            var a11 = v1.x - v0.x;
            var a12 = w0.x - w1.x;
            var a21 = v1.z - v0.z;
            var a22 = w0.z - w1.z;
            var b1 = w0.x - v0.x;
            var b2 = w0.z - v0.z;

            var detA = a11 * a22 - a12 * a21;
            if (detA != 0)
            {
                var detA1 = b1 * a22 - b2 * a12;
                var detA2 = a11 * b2 - b1 * a21;

                var s = detA1 / detA;
                var t = detA2 / detA;

                if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
                    return true;
            }
            return false;
        }

        public static bool IntersectionXZ(Vector3 v0, Vector3 v1, Vector3 w0, Vector3 w1, bool lineSegment, out Vector3 point)
        {
            point = Vector3.zero;

            // x-z plane
            var a11 = v1.x - v0.x;
            var a12 = w0.x - w1.x;
            var a21 = v1.z - v0.z;
            var a22 = w0.z - w1.z;
            var b1 = w0.x - v0.x;
            var b2 = w0.z - v0.z;

            var detA = a11 * a22 - a12 * a21;
            if (detA != 0)
            {
                var detA1 = b1 * a22 - b2 * a12;
                var detA2 = a11 * b2 - b1 * a21;

                var s = detA1 / detA;
                var t = detA2 / detA;

                point = v0 + s * (v1 - v0);

                if (!lineSegment || (s >= 0 && s <= 1 && t >= 0 && t <= 1))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IntersectsXZ(Vector3 v0, Vector3 v1, IList<Vector3> path)
        {
            if (path.Count > 1)
            {
                for (int j = 0; j < path.Count - 1; j++)
                {
                    if (IntersectsXZ(v0, v1, path[j], path[j + 1]))
                        return true;
                }
            }

            return false;
        }

        public static Vector3 GetRightNormalXZ(Vector3 p0, Vector3 p1)
        {
            var dir = p1 - p0;
            return new Vector3(dir.z, dir.y, -dir.x).normalized;
        }

        public static List<Vector3> IntersectionsXZ(Vector3 v0, Vector3 v1, IList<Vector3> path, bool isClosed = true)
        {
            var intersections = new List<Vector3>();
            if (path.Count < 2)
                return intersections;
            Vector3 pt;
            for (int j = 0; j < path.Count - 1; j++)
            {
                if (IntersectionXZ(v0, v1, path[j], path[j + 1], true, out pt))
                    intersections.Add(pt);
            }

            if (isClosed && IntersectionXZ(v0, v1, path[path.Count - 1], path[0], true, out pt))
                intersections.Add(pt);

            return intersections;
        }

        public static float GetLength(this IList<Vector3> vertices, bool isClosed = true)
        {
            var l = 0f;

            for (int i = 0; i < vertices.Count - 1; i++)
                l += Vector3.Distance(vertices[i], vertices[i + 1]);

            if (isClosed && vertices.Count > 2)
                l += Vector3.Distance(vertices[vertices.Count - 1], vertices[0]);

            return l;
        }

        public static Tuple<Vector3, float> ClosestPointOnPath(this IList<Vector3> path, Vector3 pos)
        {
            var min = float.MaxValue;
            Vector3 minPoint = new Vector3();
            Vector3 closestPoint;
            for (int i = 0; i < path.Count - 1; i++)
            {
                var d = pos.DistanceToLineSegment(path[i], path[i + 1], out closestPoint);
                if (d < min)
                {
                    min = d;
                    minPoint = closestPoint;
                }
            }

            return new Tuple<Vector3, float>(minPoint, min);
        }

        public static Vector3 CenterOfMass(ICollection<Vector3> vertices)
        {
            var com = new Vector3();
            foreach (var item in vertices)
                com += item;

            com /= vertices.Count;
            return com;
        }

        public static Vector3 Transform(this Vector3 pos, Quaternion quaternion, Vector3 translate = new Vector3())
        {
            var vec = quaternion * pos + translate;

            //quaternion = quaternion.normalized;
            ////var r = new Vector3(quaternion.x, quaternion.y, quaternion.z);
            ////var w = quaternion.w;
            ////var lhs = 2 * r;
            ////var rhs = Vector3.Cross(r, pos) + w * pos;
            ////var vec = pos + Vector3.Cross(lhs, rhs);

            //var r = new Vector3(quaternion.x, quaternion.y, quaternion.z);
            //var a = quaternion.w;
            //float theta = 0f;
            //Vector3 axis = new Vector3();

            //quaternion.ToAngleAxis(out theta, out axis);
            //r = axis.normalized * Mathf.Sin(theta);
            //a = Mathf.Cos(theta);
            //var rxp = Vector3.Cross(r, pos);
            //var first = 2 * a * rxp;
            //var second = 2 * Vector3.Cross(r, rxp);

            //var vec = pos + first + second;
            return vec;
        }

        //public static float Distance2Line2(this Vector3 pos, Vector3 a, Vector3 b, out Vector3 closetPoint, out bool isBetween)
        //{
        //    var dot = Vector3.Dot((a - pos), (b - a));
        //    //if (dot > 1) Debug.Log("extern");

        //    dot /= Vector3.Dot((b - a), (b - a));
        //    var d = ((a - pos) - dot * (b - a)).magnitude;

        //    var n = (b - a).normalized;
        //    var dot2 = Vector3.Dot((a - pos), n);
        //    var d2 = ((a - pos) - (dot2 * n)).magnitude;
        //    if (Mathf.Abs(d2 - d) > 0.001)
        //    {
        //        Debug.Log("Error 1: Vector3Utils");
        //    }
        //    if (Mathf.Abs(pos.Distance2Line(a, b) - d) > 0.001)
        //    {
        //        Debug.Log("Error 2: Vector3Utils ");
        //    }
        //    closetPoint = a - dot * (b - a);
        //    isBetween = (dot >= -1f && dot <= 0f);
        //    if (!isBetween)
        //    {
        //        var da = Vector3.Distance(a, pos);
        //        var db = Vector3.Distance(b, pos);
        //        if (da < db)
        //        {
        //            if (dot < 0)
        //            {
        //                Debug.Log("Error 3: Vector3Utils ");
        //            }
        //            closetPoint = a;
        //            d = da;
        //        }
        //        else
        //        {
        //            closetPoint = b;
        //            d = db;
        //            if (dot > -1)
        //            {
        //                Debug.Log("Error 4: Vector3Utils ");
        //            }
        //        }
        //        isBetween = true;
        //    }
        //    return d;
        //}

        public static List<Vector3> GetGridXZ(float width, float height, int resX, int resY, float posVariance, Vector2 offset = default(Vector2), float elevation = 0f)
        {
            // unity cannnot handle meshes with more than 256² vertices
            resX = Mathf.Min(resX, 256);
            resY = Mathf.Min(resY, 256);

            var dx = width / (resX - 1);
            var dy = height / (resY - 1); // y means z ;-)
            var vertices = new List<Vector3>();
            float x, y;
            float varX = dx * posVariance;
            float varY = dy * posVariance;
            var rnd = new System.Random();
            for (int i = 0; i < resY; i++)
            {
                //y = i * dy + offset.y + Random.Range(0,dy);

                for (int j = 0; j < resX; j++)
                {
                    y = i * dy + offset.y + (float)rnd.NextDouble() * varY;
                    x = j * dx + offset.x + (float)rnd.NextDouble() * varX;
                    vertices.Add(new Vector3(x, elevation, y));
                }
            }

            return vertices;
        }
    }
}