using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C用户交互
{
    public class EntityTestExam
    {

        [CommandMethod("EntityTestDemo")]
        public void EntityTestDemo()
        {
            

            Line line1 = new Line(new Point3d(0, 100, 0), new Point3d(100, 100, 0));
            Line line2 = new Line(new Point3d(50, 50, 0), new Point3d(150, 50, 0));

            line1.IsLineInserctCoincide(line2);

            Database database = HostApplicationServices.WorkingDatabase;
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

            database.AddEntitiesToModelSpace(line1,line2);
            editor.WriteMessage("ok");
        }


    }
}
