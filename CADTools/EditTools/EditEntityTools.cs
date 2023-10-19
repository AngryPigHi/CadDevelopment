using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools.EditTools
{
    public static class EditEntityTools
    {
        /// <summary>
        /// 改变图元的颜色(库中已存在图形)
        /// </summary>
        /// <param name="entityId">图元Id</param>
        /// <param name="colorIndex">颜色索引</param>
        public static void ChangeEntityColor(this ObjectId entityId, short colorIndex)
        {
            Database database = HostApplicationServices.WorkingDatabase;

            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                Entity entity = (Entity)entityId.GetObject(OpenMode.ForWrite);
                entity.ColorIndex = colorIndex;
                trans.Commit();
            }
        }

        /// <summary>
        /// 改变图元的颜色
        /// </summary>
        /// <param name="entity">图元对象</param>
        /// <param name="colorIndex">颜色索引</param>
        public static void ChangeEntityColor(this Entity entity, short colorIndex)
        {
            //判断是否是新创建，还没提交的，可修改的Entity
            if (entity.IsNewObject)
            {
                //是 可以直接修改
                entity.ColorIndex = colorIndex;
            }
            else
            {
                //不是 需要重新开启事务
                ChangeEntityColor(entity.ObjectId, colorIndex);
            }
        }

        /// <summary>
        /// 移动图形(库中已存在图形)
        /// </summary>
        /// <param name="entityId">图元的ObjectId</param>
        /// <param name="sourcePoint">参考原点</param>
        /// <param name="targetPoint">参考目标点</param>
        public static void MoveEntity(this ObjectId entityId, Point3d sourcePoint, Point3d targetPoint)
        {
            Database database = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                Entity entity = (Entity)entityId.GetObject(OpenMode.ForWrite);
                //Entity entity = (Entity)trans.GetObject(entityId,OpenMode.ForWrite);

                //变换向量
                Vector3d vector = sourcePoint.GetVectorTo(targetPoint);
                //变换矩阵（根据向量移动）
                Matrix3d matrix = Matrix3d.Displacement(vector);
                //通过变换矩阵进行转换
                entity.TransformBy(matrix);

                trans.Commit();
            }
        }

        /// <summary>
        /// 移动图形
        /// </summary>
        /// <param name="entity">图元对象</param>
        /// <param name="sourcePoint">参考原点</param>
        /// <param name="targetPoint">参考目标点</param>
        public static void MoveEntity(this Entity entity, Point3d sourcePoint, Point3d targetPoint)
        {
            if (entity.IsNewObject)
            {
                Vector3d vector = sourcePoint.GetVectorTo(targetPoint);
                Matrix3d matrix = Matrix3d.Displacement(vector);

                entity.TransformBy(matrix);
            }
            else
            {
                //通过事务重新变换
                entity.ObjectId.MoveEntity(sourcePoint, targetPoint);
            }
        }


        /// <summary>
        /// 复制图形(库中已存在图形)
        /// </summary>
        /// <param name="entityId">图元Id</param>
        /// <param name="sourcePoint">参考原点</param>
        /// <param name="targetPoint">参考目标点</param>
        /// <returns>复制出的图元</returns>
        public static Entity CopyEntity(this ObjectId entityId, Point3d sourcePoint, Point3d targetPoint)
        {
            Database database = HostApplicationServices.WorkingDatabase;
            Entity entityCopied;

            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                Entity entity = (Entity)entityId.GetObject(OpenMode.ForWrite);
                //Entity entity = (Entity)trans.GetObject(entityId,OpenMode.ForWrite);

                //变换向量
                Vector3d vector = sourcePoint.GetVectorTo(targetPoint);
                //变换矩阵（根据向量移动）
                Matrix3d matrix = Matrix3d.Displacement(vector);
                //通过变换矩阵进行转换,并拿到转换后的图元
                entityCopied = entity.GetTransformedCopy(matrix);
                blockTableRecord.AppendEntity(entityCopied);
                trans.AddNewlyCreatedDBObject(entityCopied, true);
                trans.Commit();
            }
            return entityCopied;
        }

        /// <summary>
        /// 复制图形
        /// </summary>
        /// <param name="entityId">图元</param>
        /// <param name="sourcePoint">参考原点</param>
        /// <param name="targetPoint">参考目标点</param>
        /// <returns>复制出的图元</returns>
        public static Entity CopyEntity(this Entity entity, Point3d sourcePoint, Point3d targetPoint)
        {
            Entity entityCopied;
            if (entity.IsNewObject)
            {
                Vector3d vector = sourcePoint.GetVectorTo(targetPoint);
                Matrix3d matrix = Matrix3d.Displacement(vector);

                entityCopied = entity.GetTransformedCopy(matrix);

                using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = (BlockTable)trans.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    blockTableRecord.AppendEntity(entity);
                    blockTableRecord.AppendEntity(entityCopied);
                    trans.AddNewlyCreatedDBObject(entity, true);
                    trans.AddNewlyCreatedDBObject(entityCopied, true);
                    trans.Commit();
                }
            }
            else
            {
                //通过事务重新变换
                entityCopied = entity.ObjectId.CopyEntity(sourcePoint, targetPoint);
            }
            return entityCopied;
        }

        /// <summary>
        /// 旋转图形(库中已存在图形)
        /// </summary>
        /// <param name="entityId">图元对象Id</param>
        /// <param name="centerPoint">旋转基点</param>
        /// <param name="degree">旋转角度</param>
        public static void RotateEntity(this ObjectId entityId, Point3d centerPoint, double degree)
        {
            Database database = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                Entity entity = (Entity)entityId.GetObject(OpenMode.ForWrite);
                //Entity entity = (Entity)trans.GetObject(entityId,OpenMode.ForWrite);

                //变换矩阵
                Matrix3d matrix = Matrix3d.Rotation(degree.DegreeToAngle(), Vector3d.ZAxis, centerPoint);
                //通过变换矩阵进行转换
                entity.TransformBy(matrix);

                trans.Commit();
            }
        }

        /// <summary>
        /// 旋转图形
        /// </summary>
        /// <param name="entity">图元对象</param>
        /// <param name="centerPoint">旋转基点</param>
        /// <param name="degree">旋转角度</param>
        public static void RotateEntity(this Entity entity, Point3d centerPoint, double degree)
        {
            if (entity.IsNewObject)
            {
                Matrix3d matrix = Matrix3d.Rotation(degree.DegreeToAngle(), Vector3d.ZAxis, centerPoint);
                entity.TransformBy(matrix);
            }
            else
            {
                //通过事务重新变换
                entity.ObjectId.RotateEntity(centerPoint, degree);
            }
        }


        /// <summary>
        /// 图形镜像（库中已存在图形）
        /// </summary>
        /// <param name="entityId">图元Id</param>
        /// <param name="point1">镜像第一个基点</param>
        /// <param name="point2">镜像第二个基点</param>
        /// <param name="isErase">是否擦除原图形</param>
        /// <returns>镜像后的图元</returns>
        public static Entity MirrorEntity(this ObjectId entityId, Point3d point1, Point3d point2, bool isErase)
        {
            Entity entityMirrored;

            Database database = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                Entity entity = (Entity)entityId.GetObject(OpenMode.ForWrite);
                //镜像转换的矩阵
                Matrix3d matrix = Matrix3d.Mirroring(new Line3d(point1, point2));
                //进行镜像拷贝转换
                entityMirrored = entity.GetTransformedCopy(matrix);
                blockTableRecord.AppendEntity(entityMirrored);
                trans.AddNewlyCreatedDBObject(entityMirrored, true);

                //是否擦除原来的图元
                if (isErase)
                {
                    entity.Erase();
                }
                trans.Commit();
            }
            return entityMirrored;
        }



        /// <summary>
        /// 图形镜像
        /// </summary>
        /// <param name="entity">图元对象</param>
        /// <param name="point1">镜像第一个基点</param>
        /// <param name="point2">镜像第二个基点</param>
        /// <param name="isErase">是否擦除原图形</param>
        /// <returns>镜像后的图元</returns>
        public static Entity MirrorEntity(this Entity entity, Point3d point1, Point3d point2, bool isErase)
        {
            Entity entityMirrored;
            if (entity.IsNewObject)
            {
                //构造矩阵
                Matrix3d matrix = Matrix3d.Mirroring(new Line3d(point1, point2));
                //按矩阵转换
                entityMirrored = entity.GetTransformedCopy(matrix);
                //入库
                using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = (BlockTable)trans.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    if (!isErase)
                    {
                        blockTableRecord.AppendEntity(entity);
                        trans.AddNewlyCreatedDBObject(entity, true);
                    }
                    blockTableRecord.AppendEntity(entityMirrored);
                    trans.AddNewlyCreatedDBObject(entityMirrored, true);
                    trans.Commit();
                }
            }
            else
            {
                entityMirrored = entity.ObjectId.MirrorEntity(point1, point2, isErase);
            }
            return entityMirrored;
        }


        /// <summary>
        /// 图元缩放（库中已存在图形）
        /// </summary>
        /// <param name="entityId">图元Id</param>
        /// <param name="basePoint">基点</param>
        /// <param name="factor">缩放比例</param>
        public static void ScaleEntity(this ObjectId entityId, Point3d basePoint, double factor)
        {
            using (Transaction trans = entityId.Database.TransactionManager.StartTransaction())
            {
                //构造缩放矩阵
                Matrix3d matrix = Matrix3d.Scaling(factor, basePoint);
                //拿到图元
                Entity entity = (Entity)entityId.GetObject(OpenMode.ForWrite);
                //根据矩阵进行转换
                entity.TransformBy(matrix);
                //事务提交
                trans.Commit();
            }
        }


        /// <summary>
        /// 图元缩放
        /// </summary>
        /// <param name="entity">图元</param>
        /// <param name="basePoint">基点</param>
        /// <param name="factor">缩放比例</param>
        public static void ScaleEntity(this Entity entity, Point3d basePoint, double factor)
        {
            if (entity.IsNewObject)
            {
                //构造缩放矩阵
                Matrix3d matrix = Matrix3d.Scaling(factor, basePoint);
                //根据矩阵进行转换
                entity.TransformBy(matrix);
            }
            else
            {
                entity.ObjectId.ScaleEntity(basePoint, factor);
            }
        }

        /// <summary>
        /// 删除图形
        /// </summary>
        /// <param name="entityId">图元的Id</param>
        public static void EraseEntity(this ObjectId entityId)
        {
            using (Transaction trans = entityId.Database.TransactionManager.StartTransaction())
            {
                Entity entity = (Entity)entityId.GetObject(OpenMode.ForWrite);
                entity.Erase();
                trans.Commit();//一定要提交事务
            }
        }


        /// <summary>
        /// 图形做矩形阵列（库中已存在图形）
        /// </summary>
        /// <param name="entityId">图形Id</param>
        /// <param name="rowNum">X轴方向数量</param>
        /// <param name="columnNum">Y轴方向数量</param>
        /// <param name="disRow">X轴方向间距</param>
        /// <param name="disColumn">Y轴方向间距</param>
        /// <returns>阵列生成的图形集合</returns>
        public static List<Entity> ArrayRectEntity(this ObjectId entityId, int rowNum, int columnNum, double disRow, double disColumn)
        {
            List<Entity> entityArray = new List<Entity>();

            Database database = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                Entity entity = (Entity)entityId.GetObject(OpenMode.ForWrite);

                for (int i = 0; i < rowNum; i++)
                {
                    for (int j = 0; j < columnNum; j++)
                    {
                        //构造复制的矩阵（向量变动）
                        Matrix3d matrix = Matrix3d.Displacement(new Vector3d(i * disRow, j * disColumn, 0));
                        //根据矩阵复制
                        Entity entityCopied = entity.GetTransformedCopy(matrix);
                        //添加到块表记录中
                        blockTableRecord.AppendEntity(entityCopied);
                        trans.AddNewlyCreatedDBObject(entityCopied, true);
                        //添加到结果集合中
                        entityArray.Add(entityCopied);
                    }
                }
                //将原有的擦除（会导致原图消失被替换）
                entity.Erase();

                trans.Commit();
            }

            return entityArray;
        }


        /// <summary>
        /// 图形做矩形阵列
        /// </summary>
        /// <param name="entity">图形(未入库)</param>
        /// <param name="rowNum">X轴方向数量</param>
        /// <param name="columnNum">Y轴方向数量</param>
        /// <param name="disRow">X轴方向间距</param>
        /// <param name="disColumn">Y轴方向间距</param>
        /// <returns>阵列生成的图形集合</returns>
        public static List<Entity> ArrayRectEntity(this Entity entity, int rowNum, int columnNum, double disRow, double disColumn)
        {
            List<Entity> entitiesResult = new List<Entity>();
            if (entity.IsNewObject)
            {
                Database db = HostApplicationServices.WorkingDatabase;

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    for (int i = 0; i < rowNum; i++)
                    {
                        for (int j = 0; j < columnNum; j++)
                        {
                            Matrix3d matrix = Matrix3d.Displacement(new Vector3d(i * disRow, j * disColumn, 0));
                            Entity entityCopied = entity.GetTransformedCopy(matrix);
                            btr.AppendEntity(entityCopied);
                            trans.AddNewlyCreatedDBObject(entityCopied, true);
                            entitiesResult.Add(entityCopied);
                        }
                    }
                    //entity.Erase();
                    trans.Commit();
                }
            }
            else
            {
                //已经在数据库中了，该Entity已经有ObjectId了
                entitiesResult = entity.ObjectId.ArrayRectEntity(rowNum, columnNum, disRow, disColumn);
            }

            return entitiesResult;
        }

        /// <summary>
        /// 图形做环形阵列（库中已存在entity图形）
        /// </summary>
        /// <param name="entityId">图元Id</param>
        /// <param name="num">阵列数量</param>
        /// <param name="allDegree">阵列总角度</param>
        /// <param name="centerPoint">基点</param>
        /// <returns>阵列图形集合</returns>
        public static List<Entity> ArrayPolarEntity(this ObjectId entityId, int num, double allDegree, Point3d centerPoint)
        {
            List<Entity> entitiesResult = new List<Entity>();
            Database database = HostApplicationServices.WorkingDatabase;

            //角度做重新计算（分全圆和非全圆的情况）
            allDegree = allDegree > 360 ? 360 : allDegree;
            allDegree = allDegree < -360 ? -360 : allDegree;
            int numDiv = num - 1;
            if (Math.Abs(allDegree) == 360)
            {
                numDiv = num;
            }
            //单次旋转角
            double angle = allDegree.DegreeToAngle() / numDiv;

            using (Transaction trans = database.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                Entity entity = (Entity)trans.GetObject(entityId, OpenMode.ForWrite);

                for (int i = 0; i < num; i++)
                {
                    Matrix3d matrix = Matrix3d.Rotation(i * angle, Vector3d.ZAxis, centerPoint);
                    Entity entityCopied = entity.GetTransformedCopy(matrix);
                    blockTableRecord.AppendEntity(entityCopied);
                    trans.AddNewlyCreatedDBObject(entityCopied, true);
                    entitiesResult.Add(entityCopied);
                }
                entity.Erase();
                trans.Commit();
            }

            return entitiesResult;
        }


        /// <summary>
        /// 图形做环形阵列
        /// </summary>
        /// <param name="entity">图元</param>
        /// <param name="num">阵列数量</param>
        /// <param name="allDegree">阵列总角度</param>
        /// <param name="centerPoint">基点</param>
        /// <returns>阵列图形集合</returns>
        public static List<Entity> ArrayPolarEntity(this Entity entity, int num, double allDegree, Point3d centerPoint)
        {
            List<Entity> entitiesResult = new List<Entity>();

            if (entity.IsNewObject)
            {
                Database database = HostApplicationServices.WorkingDatabase;

                //角度做重新计算（分全圆和非全圆的情况）
                allDegree = allDegree > 360 ? 360 : allDegree;
                allDegree = allDegree < -360 ? -360 : allDegree;
                int numDiv = num - 1;
                if (Math.Abs(allDegree) == 360)
                {
                    numDiv = num;
                }
                //单次旋转角
                double angle = allDegree.DegreeToAngle() / numDiv;

                using (Transaction trans = database.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = (BlockTable)trans.GetObject(database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    for (int i = 0; i < num; i++)
                    {
                        Matrix3d matrix = Matrix3d.Rotation(i * angle, Vector3d.ZAxis, centerPoint);
                        Entity entityCopied = entity.GetTransformedCopy(matrix);
                        blockTableRecord.AppendEntity(entityCopied);
                        trans.AddNewlyCreatedDBObject(entityCopied, true);
                        entitiesResult.Add(entityCopied);
                    }
                    trans.Commit();
                }
            }
            else
            {
                entitiesResult = entity.ObjectId.ArrayPolarEntity(num, allDegree, centerPoint);
            }

            return entitiesResult;
        }





    }
}
