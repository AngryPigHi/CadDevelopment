using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools.JigTools
{
    public class MoveJig : DrawJig
    {

        private Point3d jBasePoint;
        private Point3d jPrePoint;

        private List<Entity> jEntities;


        public MoveJig(Point3d basePoint, List<Entity> entities)
        {
            jBasePoint = basePoint;
            jPrePoint = basePoint;
            jEntities = entities;
        }

        //获取鼠标在屏幕的运动，需要更新图形对象的属性
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions jppOption = new JigPromptPointOptions("\r 请指定第二个点：");
            jppOption.AppendKeywordsToMessage = false;
            jppOption.BasePoint = jBasePoint;
            jppOption.UseBasePoint = true;
            jppOption.Cursor = CursorType.RubberBand;
            jppOption.UserInputControls = UserInputControls.Accept3dCoordinates;

            PromptPointResult ppResult = prompts.AcquirePoint(jppOption);
            if (ppResult.Status == PromptStatus.OK)
            {
                Point3d curPoint = ppResult.Value;
                if (curPoint != jPrePoint)
                {
                    //改变所有entity的位置
                    Vector3d vector3D = jPrePoint.GetVectorTo(curPoint);
                    Matrix3d matrix3D = Matrix3d.Displacement(vector3D);

                    using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
                    {
                        foreach (var entity in jEntities)
                        {
                            Entity temp = (Entity)entity.ObjectId.GetObject(OpenMode.ForWrite);
                            temp.TransformBy(matrix3D);
                        }
                        trans.Commit();
                    }
                }

                jPrePoint = curPoint;

            }

            return SamplerStatus.NoChange;
        }


        //重绘图形
        protected override bool WorldDraw(WorldDraw draw)
        {

            foreach (var entity in jEntities)
            {
                draw.Geometry.Draw(entity);
            }

            return true;
        }
    }
}
