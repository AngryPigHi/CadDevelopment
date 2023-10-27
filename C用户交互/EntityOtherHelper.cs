using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace C用户交互
{
    public static class EntityOtherHelper
    {




        /// <summary>
        /// 将多段线转为线段集合（不考虑曲线）
        /// </summary>
        /// <param name="polyline">多段线</param>
        /// <returns>线段集合</returns>
        public static List<Line> PloylineToLineList(this Polyline polyline)
        {
            List<Line> lines = new List<Line>();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                Point3d point1;
                Point3d point2;

                if (i == polyline.NumberOfVertices - 1)
                {
                    if (polyline.Closed)
                    {
                        point1 = polyline.GetPoint3dAt(i);
                        point2 = polyline.GetPoint3dAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    point1 = polyline.GetPoint3dAt(i);
                    point2 = polyline.GetPoint3dAt(i + 1);
                }

                Line newLine = new Line(point1, point2);
                lines.Add(newLine);
            }
            return lines;
        }

        /// <summary>
        /// 判断两个线段是否有重合部分（重合部分必须有长度）
        /// </summary>
        /// <param name="line1">第一条线</param>
        /// <param name="line2">第二条线</param>
        /// <returns></returns>
        public static bool IsLineInserctCoincide(this Line line1, Line line2)
        {
            bool result = false;

            Vector3d vector1 = line1.StartPoint.GetVectorTo(line1.EndPoint);
            Vector3d vector2 = line2.StartPoint.GetVectorTo(line2.EndPoint);

            //向量重合
            if (vector1.GetAngleTo(vector2) == 0 || vector1.GetAngleTo(vector2) == Math.PI)
            {
                //端点在线上(不含端点)
                if (line1.StartPoint.GetDistanceBetweenTwoPoints(line2.StartPoint) + line1.StartPoint.GetDistanceBetweenTwoPoints(line2.EndPoint) <= line2.Length)
                {
                    if (line1.StartPoint.GetDistanceBetweenTwoPoints(line2.StartPoint) != 0 && line1.StartPoint.GetDistanceBetweenTwoPoints(line2.EndPoint) != 0)
                    {
                        result = true;
                    }
                }
                else if (line1.EndPoint.GetDistanceBetweenTwoPoints(line2.StartPoint) + line1.EndPoint.GetDistanceBetweenTwoPoints(line2.EndPoint) <= line2.Length)
                {
                    if (line1.EndPoint.GetDistanceBetweenTwoPoints(line2.StartPoint) != 0 && line1.EndPoint.GetDistanceBetweenTwoPoints(line2.EndPoint) != 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///  判断两个多段线是否有线段重合部分（重合部分必须有长度）
        /// </summary>
        /// <param name="pLine1">第一个多段线</param>
        /// <param name="pLine2">第二个多段线</param>
        /// <returns></returns>
        public static bool IsPolylineContainCoincide(this Polyline pLine1, Polyline pLine2)
        {
            List<Line> lineList1 = pLine1.PloylineToLineList();
            List<Line> lineList2 = pLine2.PloylineToLineList();
            bool result = false;

            foreach (var line1 in lineList1)
            {
                foreach (var line2 in lineList2)
                {
                    if (line1.IsLineInserctCoincide(line2))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 获取两个点之间的距离长度
        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns>距离长度</returns>
        public static double GetDistanceBetweenTwoPoints(this Point3d point1, Point3d point2)
        {
            return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2) + Math.Pow(point2.Z - point1.Z, 2));
        }


    }
}
