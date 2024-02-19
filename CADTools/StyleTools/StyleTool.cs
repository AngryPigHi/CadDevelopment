using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools.StyleTools
{
    public static partial class StyleTool
    {

        /// <summary>
        /// 添加文字样式
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="textStyleName">新的文字样式名称</param>
        public static void AddTextStyle(this Database db, string textStyleName)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开文字样式表
                TextStyleTable tst = (TextStyleTable)trans.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                if (!tst.Has(textStyleName))
                {
                    //创建文字样式表记录
                    TextStyleTableRecord tstr = new TextStyleTableRecord();
                    tstr.Name = textStyleName;
                    tst.UpgradeOpen();//给文字样式表升级写权限
                    tst.Add(tstr);//将新建的文字样式表记录添加到文字样式表中
                    tst.DowngradeOpen();//给文字样式表权限降级
                    trans.AddNewlyCreatedDBObject(tstr, true);
                    trans.Commit();
                }
            }
        }


        /// <summary>
        /// 添加标注的样式
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="dimStyleName">标注样式的名称</param>
        /// <returns>标注样式的Id</returns>
        public static ObjectId AddDimStyle(this Database db, string dimStyleName)
        {
            ObjectId objectId = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开标注样式表
                DimStyleTable dst = (DimStyleTable)trans.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                if (!dst.Has(dimStyleName))
                {
                    //创建标注样式表记录
                    DimStyleTableRecord dstr = new DimStyleTableRecord();
                    dstr.Name = dimStyleName;
                    dst.UpgradeOpen();//给标注样式表升级写权限
                    objectId = dst.Add(dstr);// 将新建的标注样式表记录添加到标注样式表中
                    dst.DowngradeOpen();//给标注样式表权限降级
                    trans.AddNewlyCreatedDBObject(dstr, true);
                    trans.Commit();
                }
            }
            return objectId;
        }


        /// <summary>
        /// 从默认文件中加载已有的线型
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="lineTypeName">线型名称</param>
        /// <returns></returns>
        public static ObjectId LoadLineTypeFromDefaultFile(this Database db, string lineTypeName)
        {
            ObjectId lineTypeId = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开线型表
                LinetypeTable ltt = (LinetypeTable)trans.GetObject(db.LinetypeTableId, OpenMode.ForRead);
                //判断图形数据库中是否已存在指定的线型
                if (!ltt.Has(lineTypeName))
                {
                    db.LoadLineTypeFile(lineTypeName, "acad.lin");
                }
                lineTypeId = ltt[lineTypeName];
                trans.Commit();
            }
            return lineTypeId;
        }


    }
}
