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

namespace B编辑图形
{
    public class MirrorExam
    {
        [CommandMethod("MirrorDemo")]
        public void MirrorDemo()
        {
            //未入库
            Circle circle = new Circle(new Point3d(100, 100, 0), Vector3d.ZAxis, 50);
            Circle circleM1 = (Circle)circle.MirrorEntity(new Point3d(200, 0, 0), new Point3d(200, 200, 0), false);
            circleM1.ColorIndex = 6;

            Database database = HostApplicationServices.WorkingDatabase;
            database.AddEntitiesToModelSpace(circle,circleM1);
            //已在库中（删除原图）（未更新）
            Circle circleM2 = (Circle)circle.MirrorEntity(new Point3d(0, 0, 0), new Point3d(100, 0, 0), true);
            circleM2.ColorIndex = 3;
            database.AddEntitiesToModelSpace(circleM2);

            //circle.ChangeEntityColor(10);
        }

    }
}
