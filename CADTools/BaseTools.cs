using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools
{
    public static partial class BaseTools
    {
        //Angle 角度   Radian 弧度   Degree度数  Degree of Angle


        /// <summary>
        /// 角度转为弧度
        /// </summary>
        /// <param name="degree">角度</param>
        /// <returns>弧度</returns>
        public static double DegreeToAngle(this double degree)
        {
            return degree * Math.PI / 180;
        }

        /// <summary>
        /// 弧度转为角度
        /// </summary>
        /// <param name="angle">弧度</param>
        /// <returns>角度</returns>
        public static double AngleToDegree(this double angle)
        {
            return angle / Math.PI * 180;
        }

        /// <summary>
        /// 判断3个点是否在同一条直线上
        /// </summary>
        /// <param name="firstPoint">第一个点</param>
        /// <param name="secondPoint">第二个点</param>
        /// <param name="lastPoint">第三个点</param>
        /// <returns></returns>
        public static bool IsOnOneLine(this Point3d firstPoint, Point3d secondPoint, Point3d lastPoint)
        {
            Vector3d v_f2s = firstPoint.GetVectorTo(secondPoint);
            Vector3d v_f2l = firstPoint.GetVectorTo(lastPoint);
            if (v_f2s.GetAngleTo(v_f2l) == 0 || v_f2s.GetAngleTo(v_f2l) == Math.PI)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取向量与x轴夹角的弧度
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <returns>弧度（含正负）</returns>
        public static double GetRadianToXAxis(this Point3d startPoint, Point3d endPoint)
        {
            Vector3d v_s2e = startPoint.GetVectorTo(endPoint);
            Vector3d v_xAxis = new Vector3d(1, 0, 0);
            double radian = v_s2e.Y > 0 ? v_s2e.GetAngleTo(v_xAxis) : -v_s2e.GetAngleTo(v_xAxis);
            return radian;
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

        /// <summary>
        /// 获取两个点之间的中点
        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns>中点</returns>
        public static Point3d GetCenterPointBetweenTwoPoints(this Point3d point1, Point3d point2)
        {
            return new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
        }

    }
}
