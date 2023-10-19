using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADTools;
using CADTools.EditTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace B编辑图形
{
    public class CopyExam
    {

        [CommandMethod("CopyDemo")]
        public void CopyDemo()
        {
            Circle circle = new Circle(new Point3d(100, 100, 0), Vector3d.ZAxis, 100);

            //复制图元
            Entity c1 = circle.CopyEntity(new Point3d(0, 0, 0), new Point3d(-100, 0, 0));
            c1.ColorIndex = 3;
            Entity c2 = circle.CopyEntity(new Point3d(0, 0, 0), new Point3d(0, 100, 0));
            c2.ColorIndex = 6;

            Database database = HostApplicationServices.WorkingDatabase;
            database.AddEntitiesToModelSpace(circle, c1, c2);
        }
    }
}
