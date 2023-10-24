using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C用户交互
{
    public class SelectExam
    {

        [CommandMethod("SelectDemo")]
        public void SelectDemo()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

            //PromptSelectionResult psR = editor.SelectAll();

            PromptSelectionResult psR = editor.GetSelection();
            if (psR.Status == PromptStatus.OK)
            {
                ObjectId[] ids = psR.Value.GetObjectIds();
                ChangeColors(ids);
            }
        }


        private void ChangeColors(ObjectId[] ids)
        {
            Database database = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                foreach (var id in ids)
                {
                    Entity entity = (Entity)id.GetObject(OpenMode.ForWrite);
                    entity.ColorIndex = 3;
                }
                trans.Commit();
            }
        }


        [CommandMethod("SelectDemo1")]
        public void SelectDemo1()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

            //DxfCode 为枚举 
            TypedValue[] typeValues = new TypedValue[] {
                new TypedValue((int)DxfCode.Start,"Circle")
            };
            SelectionFilter filter = new SelectionFilter(typeValues);
            PromptSelectionResult psR = editor.GetSelection(filter);
            if (psR.Status == PromptStatus.OK)
            {
                ObjectId[] ids = psR.Value.GetObjectIds();
                ChangeColors(ids);
            }

        }



    }
}
