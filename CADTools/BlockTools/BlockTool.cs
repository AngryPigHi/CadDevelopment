using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools.BlockTools
{
    public static partial class BlockTool
    {

        /// <summary>
        /// 创建块表记录
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="btrName">块表记录的名称</param>
        /// <param name="entities">块表记录中的实体</param>
        /// <returns></returns>
        public static ObjectId AddBlockTableRecord(this Database db, string btrName, List<Entity> entities)
        {
            ObjectId objectId = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                if (!bt.Has(btrName))
                {
                    BlockTableRecord btr = new BlockTableRecord();//创建块表记录
                    btr.Name = btrName;
                    foreach (var entity in entities)
                    {
                        btr.AppendEntity(entity);
                    }
                    bt.UpgradeOpen();//给bt升级权限
                    bt.Add(btr);//将新创建的块表记录添加到块表中
                    bt.DowngradeOpen();//给bt降权限

                    trans.AddNewlyCreatedDBObject(btr, true);
                }
                objectId = bt[btrName];
                trans.Commit();
            }
            return objectId;
        }


        /// <summary>
        /// 插入块表记录
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="blockReferenceId">块表记录的ID</param>
        /// <param name="position">插入位置</param>
        /// <param name="rotation">旋转角度</param>
        /// <param name="scale">缩放比例</param>
        /// <returns>块参照的ID</returns>
        public static ObjectId InsertBlockReference(this Database db, ObjectId blockReferenceId, Point3d position, double rotation, Scale3d scale)
        {
            ObjectId brId = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);

                BlockReference blockReference = new BlockReference(position, blockReferenceId);
                blockReference.Position = position;//锚点位置
                blockReference.Rotation = rotation;//放大n倍
                blockReference.ScaleFactors = scale;//旋转角度

                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                brId = btr.AppendEntity(blockReference);//添加块参照的实体

                trans.AddNewlyCreatedDBObject(blockReference, true);
                trans.Commit();
            }
            return brId;
        }


        /// <summary>
        /// 插入块表记录（包含属性块参照）
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="blockReferenceId">块表记录的ID</param>
        /// <param name="position">插入位置</param>
        /// <param name="rotation">旋转角度</param>
        /// <param name="scale">缩放比例</param>
        /// <returns>块参照的ID</returns>
        public static ObjectId InsertAttributeBlockReference(this Database db, ObjectId blockReferenceId, Point3d position, double rotation, Scale3d scale)
        {
            ObjectId brId = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btrModelSpace = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);


                //1.插入块参照（只插入图形）
                BlockReference blockReference = new BlockReference(position, blockReferenceId);
                blockReference.Position = position;//锚点位置
                blockReference.Rotation = rotation;//放大n倍
                blockReference.ScaleFactors = scale;//旋转角度
                brId = btrModelSpace.AppendEntity(blockReference);//添加块参照的实体 此代码的位置不能放到插入属性块的后面 不然会报无数据库的错误


                //2.为块参照插入属性块参照
                BlockTableRecord btrCurrent = (BlockTableRecord)blockReferenceId.GetObject(OpenMode.ForRead);
                if (btrCurrent.HasAttributeDefinitions)//判断是否有属性块的定义
                {
                    foreach (ObjectId recordId in btrCurrent)//遍历块表记录中的实体ID（属性块定义继承DbText，也是Entity）
                    {
                        if (recordId.GetObject(OpenMode.ForRead) is AttributeDefinition attrDef)//判断实体是否能转为AttributeDefinition
                        {
                            AttributeReference attrRef = new AttributeReference();//创建属性块参照
                            attrRef.SetAttributeFromBlock(attrDef, blockReference.BlockTransform);//为属性块参照设置属性块定义以及块参照的变换矩阵（属性块参照跟随块参照一起变换）
                            blockReference.AttributeCollection.AppendAttribute(attrRef);//为块参照的属性块集合添加属性块
                            trans.AddNewlyCreatedDBObject(attrRef, true);//更快速显示
                        }
                    }
                }

                trans.AddNewlyCreatedDBObject(blockReference, true);
                trans.Commit();
            }
            return brId;
        }


        /// <summary>
        /// 更新块参照的属性值
        /// </summary>
        /// <param name="blockRefId">块参照的ID</param>
        /// <param name="attrNameValues">修改值的字典（Tag,TextString）</param>
        public static void UpdateBlockAttr(this ObjectId blockRefId, Dictionary<string, string> attrNameValues)
        {
            using (Transaction trans = blockRefId.Database.TransactionManager.StartTransaction())
            {
                if (blockRefId != ObjectId.Null)
                {
                    //获取块参照
                    BlockReference br = (BlockReference)blockRefId.GetObject(OpenMode.ForRead);
                    //遍历块参照中的属性块
                    foreach (ObjectId attrRefId in br.AttributeCollection)
                    {
                        AttributeReference attrRef = (AttributeReference)attrRefId.GetObject(OpenMode.ForWrite);
                        //判断是否在修改字典中，需要修改
                        if (attrNameValues.ContainsKey(attrRef.Tag))
                        {
                            attrRef.TextString = attrNameValues[attrRef.Tag];//修改TextString的值
                        }
                    }
                }
                trans.Commit();
            }
        }


    }
}
