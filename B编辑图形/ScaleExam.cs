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
    public class ScaleExam
    {

        [CommandMethod("ScaleDemo")]
        public void ScaleDemo()
        {
            Database database = HostApplicationServices.WorkingDatabase;

            ObjectId recId = database.AppendRectAngleToModelSpace(new Point2d(100, 100), new Point2d(300, 200));
            recId.ScaleEntity(new Point3d(100,100,0),2);

            //recId.EraseEntity();
        }

        
    }
}
