using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADTools;
using Autodesk.AutoCAD.DatabaseServices;
using CADTools.PromptTools;
using CADTools.EditTools;

namespace C用户交互
{
    public class PromptExam
    {
        [CommandMethod("PromptDemo")]
        public void PromptDemo()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            Database database = HostApplicationServices.WorkingDatabase;
            PromptPointResult ppr = editor.GetPoint($"\r\n请输入第一个点：");
            if (ppr.Status == PromptStatus.OK)
            {
                Point3d startPoint = ppr.Value;
                ppr = editor.GetPoint($"\r\n请输入第二个点：");
                if (ppr.Status == PromptStatus.OK)
                {
                    Point3d endPoint = ppr.Value;
                    database.AppendLineToModelSpace(startPoint, endPoint);

                }
            }
        }


        [CommandMethod("PromptDemo1")]
        public void PromptDemo1()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            Database database = Application.DocumentManager.MdiActiveDocument.Database;

            Point3d point1 = new Point3d(0, 0, 0);//默认值 空格默认输入
            Point3d point2 = new Point3d();

            //输入第一个点的配置
            PromptPointOptions ppOption = new PromptPointOptions($"\r\n请输入第一个点：");
            ppOption.AllowNone = true;//允许为空
            PromptPointResult ppResult = editor.GetPoint(ppOption);
            if (ppResult.Status == PromptStatus.Cancel)
            {
                return;//取消了就结束方法
            }

            if (ppResult.Status == PromptStatus.OK)
            {
                point1 = ppResult.Value;
            }

            //输入第二个点的配置
            ppOption = new PromptPointOptions($"\r\n请输入第二个点：");
            ppOption.AllowNone = true;
            ppOption.BasePoint = point1;//以第一个点作为基点，有辅助的虚线
            ppOption.UseBasePoint = true;

            ppResult = editor.GetPoint(ppOption);
            if (ppResult.Status == PromptStatus.Cancel || ppResult.Status == PromptStatus.None)
            {
                return;
            }
            if (ppResult.Status == PromptStatus.OK)
            {
                point2 = ppResult.Value;

                database.AppendLineToModelSpace(point1, point2);
            }

        }



        [CommandMethod("MyLine")]
        public void MyLine()
        {
            //模仿Line的命令
            Database database = Application.DocumentManager.MdiActiveDocument.Database;
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

            //报错线段的字典表
            List<Line> lineList = new List<Line>();


            //第一个点
            Point3d pointFirst = new Point3d(100, 100, 0);//默认值
            //前一个点
            Point3d pointPre = pointFirst;
            //后一个点
            Point3d pointNext = pointFirst;

            PromptPointOptions ppOption = new PromptPointOptions("\n指定第一个点:");
            ppOption.AllowNone = true;

            PromptPointResult ppResult = editor.GetPoint(ppOption);
            if (ppResult.Status == PromptStatus.Cancel) return;
            if (ppResult.Status == PromptStatus.None) pointPre = pointFirst;
            if (ppResult.Status == PromptStatus.OK)
            {
                pointFirst = ppResult.Value;
                pointPre = pointFirst;
            }

            //下一个点（多线段一起画）
            while (true)
            {
                if (lineList.Count > 1)
                {
                    //超过一条线就可以闭合了
                    ppResult = editor.GetPoint("\n指定下一点或 [闭合(C)/放弃(U)]:", pointPre, new string[] { "U", "C" });
                }
                else
                {
                    ppResult = editor.GetPoint("\n指定下一点或 [放弃(U)]:", pointPre, new string[] { "U" });
                }


                if (ppResult.Status == PromptStatus.Cancel) return;
                if (ppResult.Status == PromptStatus.None) return;
                if (ppResult.Status == PromptStatus.OK)
                {
                    pointNext = ppResult.Value;

                    Line line = new Line(pointPre, pointNext);
                    database.AddEntityToModelSpace(line);
                    lineList.Add(line);

                    pointPre = pointNext;
                }
                //输入关键词
                if (ppResult.Status == PromptStatus.Keyword)
                {
                    switch (ppResult.StringResult)
                    {
                        case "U":
                            if (lineList.Count > 0)
                            {
                                Line lastLine = lineList.Last<Line>();
                                pointPre = lastLine.StartPoint;
                                lineList.Remove(lastLine);
                                lastLine.ObjectId.EraseEntity();
                            }
                            else
                            {
                                if (pointPre == pointFirst)
                                {
                                    //只剩第一个点了,再次U
                                    return;
                                }
                                pointPre = pointFirst;
                            }
                            break;
                        case "C":
                            //首尾相连
                            Line firstLineEle = lineList.First();
                            Line lastLineEle = lineList.Last();
                            database.AddEntityToModelSpace(new Line(lastLineEle.EndPoint, firstLineEle.StartPoint));
                            return;
                    }
                }
            }


        }


        [CommandMethod("CircleDemo")]
        public void CircleDemo()
        {

            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            Database database = HostApplicationServices.WorkingDatabase;

            Point3d centerPoint = new Point3d();
            double radius = 0;

            PromptPointResult ppr = editor.GetPoint("\n 选择圆心：");
            if (ppr.Status == PromptStatus.OK)
            {
                centerPoint = ppr.Value;
            }

            PromptDistanceOptions pdOption = new PromptDistanceOptions("\n 请确认半径：");
            pdOption.BasePoint = centerPoint;
            pdOption.UseBasePoint = true;
            PromptDoubleResult pdResult = editor.GetDistance(pdOption);

            if (pdResult.Status == PromptStatus.OK)
            {
                radius = pdResult.Value;
            }

            Circle circle = new Circle(centerPoint,Vector3d.ZAxis,radius);
            database.AddEntityToModelSpace(circle);
        }



    }



}
