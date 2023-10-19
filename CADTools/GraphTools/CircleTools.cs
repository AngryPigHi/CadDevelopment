using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADTools;

namespace CADTools.GraphTools
{
    public static class CircleTools
    {
        /// <summary>
        /// 通过两点确定一个圆（直径上两点）
        /// </summary>
        /// <param name="database">文档数据库</param>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AppendCircleToModelSpace(this Database database, Point3d point1, Point3d point2)
        {
            Point3d centerPoint = point1.GetCenterPointBetweenTwoPoints(point2);
            double radius = point1.GetDistanceBetweenTwoPoints(point2) / 2.0;
            return database.AddEntityToModelSpace(new Circle(centerPoint, new Vector3d(0, 0, 1), radius));
        }

        /// <summary>
        /// 通过圆上三个点确定一个圆
        /// </summary>
        /// <param name="database">文档数据库</param>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <param name="point3">第三个点</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AppendCircleToModelSpace(this Database database, Point3d point1, Point3d point2, Point3d point3)
        {
            //判断是否在一条直线上
            if (point1.IsOnOneLine(point2, point3))
            {
                return ObjectId.Null;
            }

            //利用三点画圆弧
            CircularArc3d circularArc3D = new CircularArc3d(point1, point2, point3);

            return database.AddEntityToModelSpace(new Circle(circularArc3D.Center, new Vector3d(0, 0, 1), circularArc3D.Radius));
        }

    }
}
