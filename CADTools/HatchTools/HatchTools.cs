using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools.HatchTools
{
    public static class HatchTools
    {
        /// <summary>
        /// 填充图案名称
        /// </summary>
        public struct HatchPatternName
        {
            public static readonly string solid = "SOLID";
            public static readonly string angle = "ANGLE";
            public static readonly string ansi31 = "ANSI31";
            public static readonly string ansi32 = "ANSI32";
            public static readonly string ansi33 = "ANSI33";
            public static readonly string ansi34 = "ANSI34";
            public static readonly string ansi35 = "ANSI35";
            public static readonly string ansi36 = "ANSI36";
            public static readonly string ansi37 = "ANSI37";
            public static readonly string ansi38 = "ANSI37";
            public static readonly string arb816 = "AR-B816";
            public static readonly string arb816C = "AR-B816C";
            public static readonly string arb88 = "AR-B88";
            public static readonly string arbrelm = "AR-BRELM";
            public static readonly string arbrstd = "AR-BRSTD";
            public static readonly string arconc = "AR-CONC";

        }

        /// <summary>
        /// 渐变填充图案名称
        /// </summary>
        public struct HatchGradientName
        {
            public static readonly string gr_linear = "Linear";
            public static readonly string gr_cylinder = "Cylinder";
            public static readonly string gr_invcylinder = "Invcylinder";
            public static readonly string gr_spherical = "Spherical";
            public static readonly string gr_hemisperical = "Hemispherical";
            public static readonly string gr_curved = "Curved";
            public static readonly string gr_invsperical = "Invspherical";
            public static readonly string gr_invhemisperical = "Invhemispherical";
            public static readonly string gr_invcurved = "Invcurved";
        }


        /// <summary>
        /// 填充实体
        /// </summary>
        /// <param name="database">文档数据库</param>
        /// <param name="patternName">填充图案名称</param>
        /// <param name="scale">比例</param>
        /// <param name="degree">旋转角度</param>
        /// <param name="bgColor">背景颜色</param>
        /// <param name="hatchColorIndex">前景颜色，线条颜色，传递整数值</param>
        /// <param name="entityId">被填充实体Id</param>
        /// <returns>ObjectId</returns>
        public static ObjectId HatchEntity(this Database database, string patternName, double scale, double degree, Color bgColor, int hatchColorIndex, ObjectId entityId)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            ObjectId hatchId = ObjectId.Null;
            ids.Add(entityId);

            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                //声明图案填充对象
                Hatch hatch = new Hatch();

                //填充的参数配置
                hatch.PatternScale = scale;//显示比例
                hatch.SetHatchPattern(HatchPatternType.PreDefined, patternName);//填充类型和图案名称
                hatch.PatternAngle = degree.DegreeToAngle();//填充角度
                hatch.BackgroundColor = bgColor;//背景颜色
                hatch.ColorIndex = hatchColorIndex;//前景颜色

                BlockTable bt = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                hatchId = btr.AppendEntity(hatch);

                hatch.Associative = true;//是否关联
                hatch.AppendLoop(HatchLoopTypes.Outermost, ids);//设置边界图形和填充方式

                //计算填充并显示
                hatch.EvaluateHatch(true);

                trans.AddNewlyCreatedDBObject(hatch, true);
                trans.Commit();
            }

            return hatchId;
        }


        /// <summary>
        /// 填充实体(多图形构建的区域填充)
        /// </summary>
        /// <param name="database">文档数据库</param>
        /// <param name="patternName">填充图案名称</param>
        /// <param name="scale">比例</param>
        /// <param name="degree">旋转角度</param>
        /// <param name="bgColor">背景颜色</param>
        /// <param name="hatchColorIndex">前景颜色，线条颜色，传递整数值</param>
        /// <param name="entities">实体及填充范围</param>
        /// <returns>ObjectId</returns>
        public static ObjectId HatchEntities(this Database database, string patternName, double scale, double degree, Color bgColor, int hatchColorIndex, Dictionary<ObjectId, HatchLoopTypes> entities)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            ObjectId hatchId = ObjectId.Null;

            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                //声明图案填充对象
                Hatch hatch = new Hatch();

                //填充的参数配置
                hatch.PatternScale = scale;//显示比例
                hatch.SetHatchPattern(HatchPatternType.PreDefined, patternName);//填充类型和图案名称
                hatch.PatternAngle = degree.DegreeToAngle();//填充角度
                hatch.BackgroundColor = bgColor;//背景颜色
                hatch.ColorIndex = hatchColorIndex;//前景颜色

                BlockTable bt = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                hatchId = btr.AppendEntity(hatch);

                hatch.Associative = true;//是否关联

                //遍历实体及填充范围
                foreach (var entity in entities)
                {
                    ids.Clear();//需要清理，否则报错
                    ids.Add(entity.Key);
                    hatch.AppendLoop(entity.Value, ids);//设置边界图形和填充方式
                }

                //计算填充并显示
                hatch.EvaluateHatch(true);

                trans.AddNewlyCreatedDBObject(hatch, true);
                trans.Commit();
            }

            return hatchId;
        }


        /// <summary>
        /// 填充图案（渐变色）
        /// </summary>
        /// <param name="database">文档数据库</param>
        /// <param name="colorIndex1">颜色索引1</param>
        /// <param name="colorIndex2">颜色索引2</param>
        /// <param name="hatchGradientName">填充图案类型</param>
        /// <param name="entityId">被填充对象</param>
        /// <returns>ObjectId</returns>
        public static ObjectId HatchGradient(this Database database, short colorIndex1, short colorIndex2, string hatchGradientName, ObjectId entityId)
        {
            ObjectId hatchId = ObjectId.Null;
            ObjectIdCollection ids = new ObjectIdCollection();
            ids.Add(entityId);

            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                Hatch hatch = new Hatch();
                //设置填充类型为渐变填充
                hatch.HatchObjectType = HatchObjectType.GradientObject;
                //设置填充渐变图案类型
                hatch.SetGradient(GradientPatternType.PreDefinedGradient, hatchGradientName);
                //设置填充渐变颜色
                Color color1 = Color.FromColorIndex(ColorMethod.ByColor, colorIndex1);
                Color color2 = Color.FromColorIndex(ColorMethod.ByColor, colorIndex2);
                GradientColor gColor1 = new GradientColor(color1, 0);
                GradientColor gColor2 = new GradientColor(color2, 1);
                hatch.SetGradientColors(new GradientColor[] { gColor1, gColor2 });

                BlockTable bt = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                hatchId = btr.AppendEntity(hatch);

                hatch.Associative = true;//是否关联
                hatch.AppendLoop(HatchLoopTypes.Outermost, ids);//设置边界图形和填充方式

                //计算填充并显示
                hatch.EvaluateHatch(true);

                trans.AddNewlyCreatedDBObject(hatch, true);
                trans.Commit();
            }

            return hatchId;
        }





    }
}
