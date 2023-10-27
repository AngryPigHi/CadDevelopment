using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADTools.JigTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C用户交互
{
    public class DefineMoveExam
    {

        [CommandMethod("MoveDemo", CommandFlags.UsePickSet)]
        public void MoveDemo()
        {
            Database database = HostApplicationServices.WorkingDatabase;
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult psResult = editor.SelectImplied();
            if (psResult.Status != PromptStatus.OK)
            {
                psResult = editor.GetSelection();
            }
            if (psResult.Status != PromptStatus.OK)
            {
                return;
            }
            //指定基点
            Point3d pointBase = new Point3d(0, 0, 0);
            PromptPointOptions ppOption = new PromptPointOptions("\r 请指定基点：");
            ppOption.AllowNone = true;
            PromptPointResult ppResult = editor.GetPoint(ppOption);
            if (ppResult.Status == PromptStatus.Cancel) return;
            if (ppResult.Status == PromptStatus.OK)
            {
                pointBase = ppResult.Value;
            }


            ObjectId[] selectIds = psResult.Value.GetObjectIds();
            List<Entity> entities = GetEntities(selectIds);
            LowEntityColor(entities);

            //实现拖拽效果
            MoveJig moveJig = new MoveJig(pointBase, entities);
            editor.Drag(moveJig);

        }


        private List<Entity> GetEntities(ObjectId[] ids)
        {
            List<Entity> entities = new List<Entity>();
            Database database = HostApplicationServices.WorkingDatabase;

            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    Entity entity = (Entity)ids[i].GetObject(OpenMode.ForRead);
                    entities.Add(entity);
                }
                trans.Commit();
            }

            return entities;
        }


        private void LowEntityColor(List<Entity> entities)
        {
            Database database = HostApplicationServices.WorkingDatabase;

            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                foreach (var entity in entities)
                {
                    Entity modify = (Entity)entity.ObjectId.GetObject(OpenMode.ForWrite);
                    modify.ColorIndex = 211;
                }
                trans.Commit();
            }
        }




    }
}
