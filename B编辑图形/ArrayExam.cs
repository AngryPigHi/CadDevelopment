using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADTools.EditTools;
using CADTools;
using CADTools.GraphTools;

namespace B编辑图形
{
    public class ArrayExam
    {

        [CommandMethod("ArrayRectDemo")]
        public void ArrayRectDemo()
        {
            Circle circle = new Circle(new Point3d(100, 100, 0), Vector3d.ZAxis, 50);
            Database db = HostApplicationServices.WorkingDatabase;
            db.AddEntityToModelSpace(circle);
            //circle已入库
            List<Entity> entities = circle.ObjectId.ArrayRectEntity(4, 3, 100, 200);
        }


        [CommandMethod("ArrayRectDemo1")]
        public void ArrayRectDemo1()
        {
            Circle circle = new Circle(new Point3d(100, 100, 0), Vector3d.ZAxis, 50);
            //circle未入库
            List<Entity> entities = circle.ArrayRectEntity(4, 5, -100, -200);

        }

        [CommandMethod("ArrayPolarDemo")]
        public void ArrayPolarDemo()
        {
            Circle circle = new Circle(new Point3d(100, 100, 0), Vector3d.ZAxis, 50);
            Database db = HostApplicationServices.WorkingDatabase;
            db.AddEntityToModelSpace(circle);
            //circle已入库
            circle.ObjectId.ArrayPolarEntity(5, 360, new Point3d(100, 200, 0));
        }

        [CommandMethod("ArrayPolarDemo1")]
        public void ArrayPolarDemo1()
        {
            Circle circle = new Circle(new Point3d(100, 100, 0), Vector3d.ZAxis, 50);
            //circle未入库
            circle.ArrayPolarEntity(6, 330, new Point3d(100, 200, 0));
        }

    }
}
