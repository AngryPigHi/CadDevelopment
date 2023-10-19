using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B编辑图形
{
    public class MoveExam
    {
        [CommandMethod("MoveDemo")]
        public void MoveDemo()
        {
            Database database = HostApplicationServices.WorkingDatabase;

            Circle circle = new Circle(new Point3d(100, 100, 0), Vector3d.ZAxis, 100);

            Circle circle1 = new Circle(new Point3d(100, 100, 0), Vector3d.ZAxis, 100);
            circle1.ColorIndex = 3;

            //基准点移动
            Point3d p1 = new Point3d(50, 50, 0);
            Point3d p2 = new Point3d(50, 150, 0);
            Vector3d vector = p1.GetVectorTo(p2);
            /*circle1.Center = new Point3d(circle1.Center.X + (p2.X - p1.X), circle1.Center.Y + (p2.Y - p1.Y), 0);*/

            //生成【变换矩阵】
            Matrix3d matrix = Matrix3d.Displacement(vector);
            //通过矩阵来进行图形的变换
            circle1.TransformBy(matrix);

            database.AddEntitiesToModelSpace(circle, circle1);
        }




    }
}
