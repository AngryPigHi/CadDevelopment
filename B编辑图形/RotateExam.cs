using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADTools;
using CADTools.EditTools;
using CADTools.GraphTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace B编辑图形
{
    public class RotateExam
    {
        [CommandMethod("RotateDemo")]
        public void RotateDemo()
        {
            Database database = HostApplicationServices.WorkingDatabase;
            ObjectId recId = database.AppendRectAngleToModelSpace(new Point2d(100, 100), new Point2d(300, 200));

            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                Entity rec = (Entity)recId.GetObject(OpenMode.ForRead);
                //trans.Commit();
            }

            recId.RotateEntity(new Point3d(100, 100, 0), 30);
        }
    }
}
