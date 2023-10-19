using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools
{
    public static class LineTool
    {
        /// <summary>
        /// 通过线长和角度创建直线，并加到数据库中
        /// </summary>
        /// <param name="database">文档的数据库</param>
        /// <param name="startPoint">起点</param>
        /// <param name="length">线长</param>
        /// <param name="angle">角度 如30度 60度</param>
        public static ObjectId AppendLineToModelSpace(this Database database, Point3d startPoint, double length, double angle)
        {
            double endX = startPoint.X + length * Math.Cos(BaseTools.DegreeToAngle(angle));
            double endY = startPoint.Y + length * Math.Sin(BaseTools.DegreeToAngle(angle));
            Point3d endPoint = new Point3d(endX, endY, 0);
            Line line = new Line(startPoint, endPoint);
            return database.AddEntityToModelSpace(line);
        }

        /// <summary>
        /// 两点定一个直线
        /// </summary>
        /// <param name="database">文档数据库</param>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <returns>图形Id</returns>
        public static ObjectId AppendLineToModelSpace(this Database database, Point3d startPoint, Point3d endPoint)
        {
            Line line = new Line(startPoint, endPoint);
            return database.AddEntityToModelSpace(line);
        }
    }
}
