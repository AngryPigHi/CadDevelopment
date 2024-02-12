using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools.LayerTools
{
    public static partial class LayerTool
    {
        /// <summary>
        /// 枚举：添加图形的状态
        /// </summary>
        public enum AddLayerStatus
        {
            AddLayerOk = 0,//添加成功
            NotInput = 1,//未输入字符
            IllegalLayerName = 2,//非法的图层名
            ExistLayerName = 3//该图层名已存在
        }

        /// <summary>
        /// 添加图层的返回结果
        /// </summary>
        public struct AddLayerResult
        {
            /// <summary>
            /// 添加图层的状态
            /// </summary>
            public AddLayerStatus status;
            /// <summary>
            /// 成功添加图层返回该图层的名称
            /// </summary>
            public string layerName;
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名称</param>
        public static AddLayerResult AddLayer(this Database db, string layerName)
        {
            AddLayerResult result = new AddLayerResult();

            if (string.IsNullOrEmpty(layerName))
            {
                result.status = AddLayerStatus.NotInput;
                return result;
            }

            //2.验证图层名称是否合法
            try
            {
                SymbolUtilityServices.ValidateSymbolName(layerName, false);
            }
            catch
            {
                result.status = AddLayerStatus.IllegalLayerName;
                return result;
            }

            //3.添加图层操作
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);

                if (!lt.Has(layerName))//判断是否已有该图层名
                {
                    LayerTableRecord ltr = new LayerTableRecord();//新建图层
                    ltr.Name = layerName;
                    lt.UpgradeOpen();//需要给图层表添加记录，需要给图层表升级
                    lt.Add(ltr);//添加图层记录
                    lt.DowngradeOpen();//再给图层表降级

                    trans.AddNewlyCreatedDBObject(ltr, true);//新的图层表记录是新实体
                    trans.Commit();
                    result.status = AddLayerStatus.AddLayerOk;

                }
                else
                {
                    result.status = AddLayerStatus.ExistLayerName;
                }
                result.layerName = layerName;
                return result;
            }
        }


        /// <summary>
        /// 枚举：图层的修改状态
        /// </summary>
        public enum ChangePropertyStatus
        {
            ChangeOK,
            LayerNotExist
        }

        /// <summary>
        /// 修改图层的颜色
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名称</param>
        /// <param name="colorIndex">颜色Index</param>
        /// <returns>图形属性的修改状态</returns>
        public static ChangePropertyStatus ChangeLayeColor(this Database db, string layerName, short colorIndex)
        {
            ChangePropertyStatus status;//修改状态

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                if (lt.Has(layerName))//判断是否存在该图层
                {
                    LayerTableRecord ltr = (LayerTableRecord)trans.GetObject(lt[layerName], OpenMode.ForWrite);
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, colorIndex);//修改图层的颜色
                    trans.Commit();
                    status = ChangePropertyStatus.ChangeOK;
                }
                else
                {
                    status = ChangePropertyStatus.LayerNotExist;
                }
            }
            return status;
        }

        /// <summary>
        /// 设置某图层为当前图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">目标图层名称</param>
        /// <returns>是否设置成功</returns>
        public static bool SetCurrentLayer(this Database db, string layerName)
        {
            bool isOk = false;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);

                if (lt.Has(layerName))//判断是否有该图层名称
                {
                    ObjectId ltrId = lt[layerName];//获取目标图层的ObjectId
                    if (db.Clayer != ltrId)//判断当前图层是否是目标图层
                    {
                        db.Clayer = ltrId;//设置当前图层为目标图层
                    }
                    isOk = true;
                }
                trans.Commit();
            }
            return isOk;
        }

        /// <summary>
        /// 获取数据库中的所有的图层表记录的列表
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <returns>图层表记录的列表</returns>
        public static List<LayerTableRecord> GetAllLayers(this Database db)
        {
            List<LayerTableRecord> records = new List<LayerTableRecord>();

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                layerTable.GenerateUsageData();//更新图层表的数据，更新IsUsed的值
                foreach (var recordId in layerTable)//遍历图层表，每个元素是图层表记录的Id
                {
                    LayerTableRecord record = (LayerTableRecord)recordId.GetObject(OpenMode.ForRead);
                    records.Add(record);
                }
            }

            return records;
        }


        /// <summary>
        /// 删除某图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名称</param>
        /// <returns>是否成功删除该图层</returns>
        public static bool DeleteLayer(this Database db, string layerName)
        {
            bool isDeleteOk = false;

            //默认的图层不可删除
            if (layerName == "0" || layerName == "Defpoints")
            {
                return isDeleteOk;
            }

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                lt.GenerateUsageData();//更新图层表的数据，更新IsUsed
                if (lt.Has(layerName))
                {
                    LayerTableRecord ltr = (LayerTableRecord)trans.GetObject(lt[layerName], OpenMode.ForWrite);
                    if (!ltr.IsUsed && db.Clayer != ltr.ObjectId)//图层中没有图元的和非当前图层的图层可以删除
                    {
                        ltr.Erase();
                        isDeleteOk = true;
                    }
                }
                else
                {
                    isDeleteOk = true;//不存在的图层可以默认为删除
                }
                trans.Commit();
            }

            return isDeleteOk;
        }

        /// <summary>
        /// 删除某图层（可强制删除）
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名称</param>
        /// <param name="isForceDelete">是否强制删除</param>
        /// <returns></returns>
        public static bool DeleteLayer(this Database db, string layerName, bool isForceDelete)
        {
            bool isDeleteOk = false;

            //默认的图层不可删除
            if (layerName == "0" || layerName == "Defpoints")
            {
                return isDeleteOk;
            }

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead);
                lt.GenerateUsageData();//更新图层表的数据，更新IsUsed

                if (lt.Has(layerName))
                {
                    LayerTableRecord ltr = (LayerTableRecord)trans.GetObject(lt[layerName], OpenMode.ForWrite);

                    if (!ltr.IsUsed && db.Clayer != ltr.ObjectId)//图层中没有图元的和非当前图层的图层可以删除
                    {
                        ltr.Erase();
                    }
                    else
                    {
                        if (isForceDelete)//包含强制删除的情况
                        {
                            if (db.Clayer == ltr.ObjectId)
                            {
                                db.Clayer = lt["0"];//如果是当前图层，则将当前图层设置为默认的"0"图层
                            }
                            if (ltr.IsUsed)
                            {
                                //如果要删除的图层中还有图元，则先删除所有图元
                                ltr.DeleteAllEntityInLayer();//删除图层中的所有图元
                                ltr.Erase();
                            }
                        }
                    }
                    isDeleteOk = true;
                }
                else
                {
                    isDeleteOk = true;//不存在的图层可以默认为删除
                }
                trans.Commit();
            }

            return isDeleteOk;
        }


        /// <summary>
        /// 删除某图层中的所有图元
        /// </summary>
        /// <param name="ltr">图层表记录</param>
        public static void DeleteAllEntityInLayer(this LayerTableRecord ltr)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                TypedValue[] values = new TypedValue[] {
                    new TypedValue((int)DxfCode.LayerName,ltr.Name)
                };
                SelectionFilter filter = new SelectionFilter(values);
                PromptSelectionResult results = editor.SelectAll(filter);
                if (results.Status == PromptStatus.OK)
                {
                    ObjectId[] selectIds = results.Value.GetObjectIds();
                    foreach (ObjectId objectId in selectIds)
                    {
                        Entity entity = (Entity)objectId.GetObject(OpenMode.ForWrite);
                        entity.Erase();
                    }
                }
                trans.Commit();
            }
        }




    }
}
