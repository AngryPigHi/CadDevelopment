using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools.GraphTools
{
    public static class PolylineTools
    {
        /// <summary>
        /// 根据两点确定一个矩形
        /// </summary>
        /// <param name="database">文档数据库</param>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns></returns>
        public static ObjectId AppendRectAngleToModelSpace(this Database database, Point2d point1, Point2d point2)
        {
            //声明多段线
            Polyline rectpline = new Polyline();

            //按顺序确定四个顶点坐标
            Point2d p1 = new Point2d(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
            Point2d p2 = new Point2d(Math.Max(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
            Point2d p3 = new Point2d(Math.Max(point1.X, point2.X), Math.Max(point1.Y, point2.Y));
            Point2d p4 = new Point2d(Math.Min(point1.X, point2.X), Math.Max(point1.Y, point2.Y));

            rectpline.AddVertexAt(0, p1, 0, 0, 0);
            rectpline.AddVertexAt(1, p2, 0, 0, 0);
            rectpline.AddVertexAt(2, p3, 0, 0, 0);
            rectpline.AddVertexAt(3, p4, 0, 0, 0);

            rectpline.Closed = true;

            return database.AddEntityToModelSpace(rectpline);
        }

        /// <summary>
        /// 绘制一个正多边形
        /// </summary>
        /// <param name="database">文档数据库</param>
        /// <param name="centerPoint">中心点</param>
        /// <param name="radius">半径</param>
        /// <param name="sideNum">边的数量</param>
        /// <param name="startDegree">起始点与圆心连线的角度</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AppendRegularPolygonToModelSpace(this Database database, Point2d centerPoint, double radius, int sideNum, double startDegree)
        {
            Polyline regularPolygon = new Polyline();

            if (sideNum < 3)
            {
                return ObjectId.Null;
            }

            Point2d[] points = new Point2d[sideNum];
            //起始点的角度
            double angle = startDegree.DegreeToAngle();

            for (int i = 0; i < sideNum; i++)
            {
                //三角函数计算
                points[i] = new Point2d(centerPoint.X + radius * Math.Cos(angle), centerPoint.Y + radius * Math.Sin(angle));
                regularPolygon.AddVertexAt(i, points[i], 0, 0, 0);
                //弧度递增
                angle += 2 * Math.PI / sideNum;
            }

            regularPolygon.Closed = true;

            return database.AddEntityToModelSpace(regularPolygon);
        }


    }
}
