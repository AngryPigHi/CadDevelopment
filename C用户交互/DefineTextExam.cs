using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
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
    public class DefineTextExam
    {
        [CommandMethod("TextDemo2")]
        public void TextDemo2()
        {
            Database database = HostApplicationServices.WorkingDatabase;

            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                DBText text = new DBText();
                //text.Position = new Point3d(0, 0, 0);//文字位置
                text.TextString = "Hello CAD Development!";//文字内容
                text.Height = 100;//文字行高
                //text.HorizontalMode = TextHorizontalMode.TextCenter;//水平方向的对齐方式
                //text.VerticalMode = TextVerticalMode.TextVerticalMid;//垂直方向的对齐方式
                //text.AlignmentPoint = new Point3d(0, 0, 0);

                blockTableRecord.AppendEntity(text);
                trans.AddNewlyCreatedDBObject(text, true);

                trans.Commit();
            }
        }


        [CommandMethod("TextDemo")]
        public void TextDemo()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database database = doc.Database;

            DBText text = new DBText();
            text.Position = new Point3d(0, 0, 0);//文字位置
            text.TextString = "Hello CAD Development!";//文字内容
            text.Height = 100;//文字行高
            //设置对齐方式时候，必须设置对齐点
            text.HorizontalMode = TextHorizontalMode.TextCenter;//水平方向的对齐方式
            text.VerticalMode = TextVerticalMode.TextVerticalMid;//垂直方向的对齐方式
            text.AlignmentPoint = text.Position;//对齐的点
            database.AddEntitiesToModelSpace(text);
        }


        [CommandMethod("MTextDemo")]
        public void MTextDemo()
        {
            Database database = HostApplicationServices.WorkingDatabase;

            MText mText = new MText();
            mText.Location = new Point3d(100,100,0);//位置：左上角
            mText.Contents = "你好，CAD二次开发，可以提升工作效率！";//文字内容
            mText.Width = 15;//文字框宽度，长度达到5的时候自动换行
            mText.Height = 25;//文字框高度，超出自动换行向下显示
            mText.TextHeight = 5;//文字行高
            mText.Rotation = Math.PI / 4.0;//旋转角度

            database.AddEntitiesToModelSpace(mText);
        }

    }
}
