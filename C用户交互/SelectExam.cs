using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
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

            //过滤
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

            //框选（点选:起止同一个点）
            PromptSelectionResult psR1 = editor.SelectCrossingWindow(new Point3d(0, 0, 0), new Point3d(2, 2, 2));
            if (psR1.Status == PromptStatus.OK)
            {
                ObjectId[] ids = psR1.Value.GetObjectIds();
            }

        }



        //需要加个参数
        [CommandMethod("SelectDemo2", CommandFlags.UsePickSet)]
        public void SelectDemo2()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult psResult = editor.SelectImplied();
            if (psResult.Status==PromptStatus.OK)
            {
                //拿到已选择图形
                psResult.Value.GetObjectIds();
            }
            else if (psResult.Status==PromptStatus.Error)
            {
                //图中没有已选择的图形
            }
        }


    }
}
