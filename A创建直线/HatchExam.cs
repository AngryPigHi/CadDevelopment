using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADTools.GraphTools;
using CADTools.HatchTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A创建图形
{
    public class HatchExam
    {
        [CommandMethod("HatchDemo")]
        public void HatchDemo()
        {
            Database database = HostApplicationServices.WorkingDatabase;
            //ObjectId recId = database.AppendRectAngleToModelSpace(new Point2d(100, 100), new Point2d(500, 600));
            ObjectId cirId = database.AppendCircleToModelSpace(new Point3d(500, 100, 0), new Point3d(700, 400, 0));
            ObjectIdCollection ids = new ObjectIdCollection();
            //ids.Add(recId);
            ids.Add(cirId);

            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                //声明图案填充对象
                Hatch hatch = new Hatch();

                //填充的参数配置
                hatch.PatternScale = 5;//显示比例
                hatch.SetHatchPattern(HatchPatternType.PreDefined, "ANGLE");//填充类型和图案名称
                hatch.PatternAngle = Math.PI / 4;//填充角度



                BlockTable bt = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                btr.AppendEntity(hatch);


                hatch.Associative = true;//是否关联
                hatch.AppendLoop(HatchLoopTypes.Outermost, ids);//设置边界图形和填充方式

                //计算填充并显示
                hatch.EvaluateHatch(true);

                trans.AddNewlyCreatedDBObject(hatch, true);
                trans.Commit();
            }
        }


        [CommandMethod("HatchDemo1")]
        public void HatchDemo1()
        {
            //文档数据库
            Database database = HostApplicationServices.WorkingDatabase;
            //被填充的对象
            ObjectId recId = database.AppendRectAngleToModelSpace(new Point2d(200, 200), new Point2d(800, 600));
            //进行填充
            database.HatchEntity(HatchTools.HatchPatternName.angle, 2, 60, Color.FromColorIndex(ColorMethod.ByColor, 2), 1, recId);
        }


        [CommandMethod("HatchDemo2")]
        public void HatchDemo2()
        {
            //文档数据库
            Database database = HostApplicationServices.WorkingDatabase;
            //被填充的对象
            ObjectId c1Id = database.AppendCircleToModelSpace(new Point3d(100, 100, 0), new Point3d(300, 100, 0));
            ObjectId c2Id = database.AppendCircleToModelSpace(new Point3d(150, 100, 0), new Point3d(250, 100, 0));
            ObjectId c3Id = database.AppendCircleToModelSpace(new Point3d(180, 100, 0), new Point3d(220, 100, 0));

            Dictionary<ObjectId, HatchLoopTypes> dics = new Dictionary<ObjectId, HatchLoopTypes>();
            dics.Add(c1Id, HatchLoopTypes.Outermost);
            dics.Add(c2Id, HatchLoopTypes.Outermost);
            dics.Add(c3Id, HatchLoopTypes.Outermost);

            //获取填充
            database.HatchEntities(HatchTools.HatchPatternName.angle, 2, 60, Color.FromColorIndex(ColorMethod.ByColor, 3), 1, dics);
        }

        [CommandMethod("HatchDemo3")]
        public void HatchDemo3()
        {
            //文档数据库
            Database database = HostApplicationServices.WorkingDatabase;
            ObjectId c1Id = database.AppendCircleToModelSpace(new Point3d(100, 100, 0), new Point3d(300, 100, 0));
            database.HatchGradient(3,4,HatchTools.HatchGradientName.gr_cylinder, c1Id);
        }


    }
}
