using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools
{
    public static partial class AddEntityTools
    {
        /// <summary>
        /// 添加单个图元到文档的数据库中
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="entity">图元对象</param>
        /// <returns>被添加图元的ObjectId</returns>
        public static ObjectId AddEntityToModelSpace(this Database db, Entity entity)
        {
            ObjectId entityId = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                entityId = blockTableRecord.AppendEntity(entity);
                trans.AddNewlyCreatedDBObject(entity, true);
                trans.Commit();
            }
            return entityId;
        }

        /// <summary>
        /// 添加多个图元到文档的数据库中
        /// </summary>
        /// <param name="db">文档的数据库对象</param>
        /// <param name="entities">多个图元</param>
        /// <returns>被添加的图元的ObjectId数组</returns>
        public static ObjectId[] AddEntitiesToModelSpace(this Database db, params Entity[] entities)
        {
            ObjectId[] objectIds = new ObjectId[entities.Length];
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //块表中的模型空间是一个块表记录，这个块表记录中可以添加图元
                BlockTableRecord record = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                for (int i = 0; i < entities.Length; i++)
                {
                    objectIds[i] = record.AppendEntity(entities[i]);
                    trans.AddNewlyCreatedDBObject(entities[i], true);
                }
                trans.Commit();
            }
            return objectIds;
        }

        /// <summary>
        /// 添加图元集合到文档的数据库中
        /// </summary>
        /// <param name="db">文档的数据库</param>
        /// <param name="entities">图元集合</param>
        /// <returns>被添加的图元的ObjectId集合</returns>
        public static List<ObjectId> AddEntitiesToModelSpace(this Database db, List<Entity> entities)
        {
            List<ObjectId> objectIds = new List<ObjectId>();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //[块表]中的[模型空间]是一个[块表记录]，这个[块表记录]中可以添加[图元]
                BlockTableRecord record = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                foreach (var entity in entities)
                {
                    objectIds.Add(record.AppendEntity(entity));
                    trans.AddNewlyCreatedDBObject(entity, true);
                }
                trans.Commit();
            }
            return objectIds;
        }

    }
}
