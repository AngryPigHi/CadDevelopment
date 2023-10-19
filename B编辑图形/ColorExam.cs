using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADTools;
using Autodesk.AutoCAD.DatabaseServices;
using CADTools.GraphTools;
using Autodesk.AutoCAD.Geometry;
using CADTools.EditTools;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace B编辑图形
{
    public class ColorExam
    {

        [CommandMethod("ColorDemo")]
        public void ColorDemo()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

            Database database = HostApplicationServices.WorkingDatabase;
            ObjectId cId = database.AppendCircleToModelSpace(new Point3d(100, 100, 0), new Point3d(300, 100, 0));
            cId.ChangeEntityColor(6);

            Circle circle = new Circle(new Point3d(300, 300, 0), Vector3d.ZAxis, 100);
            circle.ChangeEntityColor(4);
            editor.WriteMessage($"circle.IsNewObject={circle.IsNewObject}，Color={circle.ColorIndex}\r\n");
            database.AddEntityToModelSpace(circle);//entity已提交
            circle.ChangeEntityColor(3);
            editor.WriteMessage($"circle.IsNewObject={circle.IsNewObject}，Color={circle.ColorIndex}");
        }

    }
}
