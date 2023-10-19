using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADTools;
using CADTools.GraphTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A创建图形
{
    public class CircleExam
    {

        [CommandMethod("CircleDemo")]
        public void CircleDemo()
        {
            Circle circle = new Circle();
            circle.Center = new Point3d(50, 50, 0);
            circle.Radius = 50;

            Circle circle1 = new Circle(new Point3d(100, 100, 0), new Vector3d(0, 0, 1), 50);

            Database database = HostApplicationServices.WorkingDatabase;
            database.AddEntitiesToModelSpace(circle1, circle);
        }

        [CommandMethod("CircleDemo1")]
        public void CircleDemo1()
        {
            Database database = HostApplicationServices.WorkingDatabase;
            database.AppendCircleToModelSpace(new Point3d(100, 0, 0), new Point3d(100, 200, 0));
        }

        [CommandMethod("CircleDemo2")]
        public void CircleDemo2()
        {
            Database database = HostApplicationServices.WorkingDatabase;
            database.AppendCircleToModelSpace(new Point3d(100, 0, 0), new Point3d(100, 200, 0),new Point3d(175,125,0));
        }
    }
}
