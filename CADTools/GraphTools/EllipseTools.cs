using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools.GraphTools
{
    public static class EllipseTools
    {
        /// <summary>
        /// 绘制椭圆
        /// </summary>
        /// <param name="db">文档数据库</param>
        /// <param name="centerPoint">圆心</param>
        /// <param name="majorRadius">长轴长度</param>
        /// <param name="shortRadius">短轴长度</param>
        /// <param name="degree">长轴与x正方向的夹角角度</param>
        /// <returns></returns>
        public static ObjectId AppendEllipseToModelSpace(this Database db, Point3d centerPoint, double majorRadius, double shortRadius, double degree)
        {
            //长轴向量
            Vector3d major_Vector = new Vector3d(majorRadius * Math.Cos(degree.DegreeToAngle()), majorRadius * Math.Sin(degree.DegreeToAngle()), 0);

            return db.AddEntityToModelSpace(new Ellipse(centerPoint, Vector3d.ZAxis, major_Vector, shortRadius / majorRadius, 0, 2 * Math.PI));
        }

        /// <summary>
        /// 画一个内接于矩形的椭圆
        /// </summary>
        /// <param name="db">文档数据库</param>
        /// <param name="point1">矩形对角点一</param>
        /// <param name="point2">矩形对角点二</param>
        /// <returns></returns>
        public static ObjectId AppendEllipseToModelSpace(this Database db, Point3d point1, Point3d point2)
        {
            //圆心
            Point3d centerPoint = point1.GetCenterPointBetweenTwoPoints(point2);
            double majorRadius = Math.Abs(point2.X - point1.X);//主轴长
            double shortRadius = Math.Abs(point2.Y - point1.Y);//短轴长

            double ratio = 0;
            Vector3d majorVector;

            if (majorRadius>=shortRadius)
            {
                ratio = shortRadius / majorRadius;
                majorVector = new Vector3d((point2.X - point1.X) / 2, 0, 0);//主轴向量 X
            }
            else
            {
                ratio = majorRadius/ shortRadius;
                majorVector = new Vector3d(0,(point2.Y - point1.Y) / 2,  0);//主轴向量 Y
            }

            //画矩形
            db.AppendRectAngleToModelSpace(new Point2d(point1.X, point1.Y), new Point2d(point2.X, point2.Y));

            //画椭圆
            return db.AddEntityToModelSpace(new Ellipse(centerPoint, Vector3d.ZAxis, majorVector, ratio, 0, 2 * Math.PI));
        }

    }
}
