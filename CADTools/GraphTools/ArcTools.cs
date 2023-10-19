using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools.GraphTools
{
    public static class ArcTools
    {
        /// <summary>
        /// 通过3点确定圆弧，并添加至数据库中
        /// </summary>
        /// <param name="database">数据库</param>
        /// <param name="startPoint">第一个点</param>
        /// <param name="onArcPoint">第二个点</param>
        /// <param name="endPoint">第三个点</param>
        public static ObjectId AppendArcToModelSpace(this Database database, Point3d startPoint, Point3d onArcPoint, Point3d endPoint)
        {
            //三点画圆弧
            CircularArc3d circularArc3D = new CircularArc3d(startPoint, onArcPoint, endPoint);

            //将三点画的圆弧转为Arc对象
            Point3d centerPoint = circularArc3D.Center;
            double radius = circularArc3D.Radius;

            /* Vector3d c2s = centerPoint.GetVectorTo(startPoint);
             Vector3d c2e = centerPoint.GetVectorTo(endPoint);
             Vector3d xVector = new Vector3d(1, 0, 0);//x正方向的向量

             //根据圆心到端点的向量，判断弧度是正负
             //弧度一般在[-π,π]之间，也就是[-180°,180°]
             double startRadian = c2s.Y > 0 ? xVector.GetAngleTo(c2s) : -xVector.GetAngleTo(c2s);
             double endRadian = c2e.Y > 0 ? xVector.GetAngleTo(c2e) : -xVector.GetAngleTo(c2e);*/

            double startRadian = centerPoint.GetRadianToXAxis(startPoint);
            double endRadian = centerPoint.GetRadianToXAxis(endPoint);

            Arc arc = new Arc(centerPoint, radius, startRadian, endRadian);

            return database.AddEntityToModelSpace(arc);
        }

        /// <summary>
        /// 通过圆心、起点、弧度角绘制圆弧
        /// </summary>
        /// <param name="database">文档数据库</param>
        /// <param name="centerPoint">圆心</param>
        /// <param name="startPoint">起点</param>
        /// <param name="degree">圆心与起点连接的向量，与此向量的夹角(角度值)，逆时针为正方向</param>
        /// <returns>图元Id</returns>
        public static ObjectId AppendArcToModelSpace(this Database database, Point3d centerPoint, Point3d startPoint, double degree)
        {
            //获得半径
            double radius = centerPoint.GetDistanceBetweenTwoPoints(startPoint);
            //获取起点弧度
            double startAngleRadian = centerPoint.GetRadianToXAxis(startPoint);
            //获取终点弧度
            double endAngleRadian = startAngleRadian + degree.DegreeToAngle();
            Arc arc = new Arc(centerPoint, radius, startAngleRadian, endAngleRadian);
            return database.AddEntityToModelSpace(arc);
        }

    }
}
