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

namespace A创建图形
{
    public class LineExam
    {
        [CommandMethod("TestDemo")]
        public void TestDemo()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            editor.WriteMessage("开始画直线了！");
        }

        [CommandMethod("LineDemo")]
        public void LineDemo()
        {

            Line line = new Line();
            Point3d startPoint = new Point3d(100, 100, 0);
            Point3d endPoint = new Point3d(100, 200, 0);
            line.StartPoint = startPoint;
            line.EndPoint = endPoint; //此处的line对象还是只存在于内存之中 并不能在界面上显示


            //声明图形数据库对象
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            //开启事务处理（涉及到文件处理，非托管资源的释放用using）
            //事务处理保证一致性
            using (Transaction trans = db.TransactionManager.StartTransaction())//数据库层面的事务
            {
                //打开块表  父类对象可以强转成子类 （父类信息少，可以转成信息多的子类）
                BlockTable blockTable = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //打开块表记录
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                //将直线添加到块表记录中
                blockTableRecord.AppendEntity(line);
                //更新数据
                trans.AddNewlyCreatedDBObject(line, true);
                //将事务提交 数据库的事务结束
                trans.Commit();
            }

            Line line2 = new Line(new Point3d(100, 200, 0), new Point3d(200, 200, 0));
            Line line3 = new Line(new Point3d(200, 200, 0), new Point3d(200, 100, 0));
            Line line4 = new Line(new Point3d(200, 100, 0), new Point3d(100, 100, 0));

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //拿到块表
                BlockTable blockTable = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //拿到块表记录
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                //添加实体
                blockTableRecord.AppendEntity(line2);
                blockTableRecord.AppendEntity(line3);
                blockTableRecord.AppendEntity(line4);
                //更新数据
                trans.AddNewlyCreatedDBObject(line2, true);
                trans.AddNewlyCreatedDBObject(line3, true);
                trans.AddNewlyCreatedDBObject(line4, true);
                //提交事务
                trans.Commit();
            }
        }


        [CommandMethod("LineDemo2")]
        public void LinwDemo2()
        {
            Line line1 = new Line(new Point3d(100, 100, 0), new Point3d(100, 200, 0));
            Line line2 = new Line(new Point3d(100, 200, 0), new Point3d(200, 200, 0));
            Line line3 = new Line(new Point3d(200, 200, 0), new Point3d(200, 100, 0));
            Line line4 = new Line(new Point3d(200, 100, 0), new Point3d(100, 100, 0));

            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            //扩展方法
            database.AddEntitiesToModelSpace(line1, line2, line3, line4);
        }

        [CommandMethod("LineAngle")]
        public void LineAngle()
        {
            Point3d startPoint = new Point3d(100, 100, 0);
            //Line line = LineTool.AppendLineFromAngle(startPoint,300,30);
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            //database.AddEntityToModelSpace(line);
            database.AppendLineToModelSpace(startPoint, 300, 30);
        }
    }
}
