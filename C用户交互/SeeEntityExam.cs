using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C用户交互
{
    public class SeeEntityExam
    {

        [CommandMethod("SeePolyline")]
        public void SeePolyline()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            Database database = HostApplicationServices.WorkingDatabase;

            //1.获取输入的字符串
            PromptStringOptions psOption = new PromptStringOptions("\n 请输入点集字符串：");
            psOption.AllowSpaces = true;

            PromptResult pResult = editor.GetString(psOption);
            if (pResult.Status == PromptStatus.OK)
            {
                //2.将字符串转为点集
                string pointsString = pResult.StringResult;
                if (!string.IsNullOrWhiteSpace(pointsString))
                {
                    List<Point2dCanSet> point2Ds = JsonConvert.DeserializeObject<List<Point2dCanSet>>(pointsString);
                    Polyline polyline = new Polyline();
                    for (int i = 0; i < point2Ds.Count; i++)
                    {
                        Point2d pointNew = new Point2d(point2Ds[i].X, point2Ds[i].Y);
                        polyline.AddVertexAt(i, pointNew, 0, 0, 0);
                    }
                    polyline.Closed = true;

                    database.AddEntityToModelSpace(polyline);
                }
            }


        }

        [CommandMethod("SeeListPolyline")]
        public void SeeListPolyline()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            Database database = HostApplicationServices.WorkingDatabase;

            //1.获取输入的字符串
            PromptStringOptions psOption = new PromptStringOptions("\n 请输入多点集字符串：");
            psOption.AllowSpaces = true;

            PromptResult pResult = editor.GetString(psOption);
            if (pResult.Status == PromptStatus.OK)
            {
                //2.将字符串转为点集
                string pointsString = pResult.StringResult;
                if (!string.IsNullOrWhiteSpace(pointsString))
                {
                    List<List<Point2dCanSet>> polylines = JsonConvert.DeserializeObject<List<List<Point2dCanSet>>>(pointsString);

                    List<Polyline> resultPolylines = new List<Polyline>();

                    foreach (var point2Ds in polylines)
                    {
                        Polyline singlePolyline = new Polyline();
                        for (int i = 0; i < point2Ds.Count; i++)
                        {
                            Point2d pointNew = new Point2d(point2Ds[i].X, point2Ds[i].Y);
                            singlePolyline.AddVertexAt(i, pointNew, 0, 0, 0);
                        }
                        singlePolyline.Closed = true;
                        resultPolylines.Add(singlePolyline);
                    }

                    foreach (var item in resultPolylines)
                    {
                        database.AddEntityToModelSpace(item);
                    }

                }
            }
        }


        [CommandMethod("SeeListPolylineFromTxt")]
        public void SeeListPolylineFromTxt()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            Database database = HostApplicationServices.WorkingDatabase;

            //1.获取输入的字符串
            PromptStringOptions psOption = new PromptStringOptions("\n 请输入文档路径：");
            psOption.AllowSpaces = true;

            PromptResult pResult = editor.GetString(psOption);
            if (pResult.Status == PromptStatus.OK)
            {
                //2.将字符串转为点集
                string filePath = pResult.StringResult;

                string pointsString = File.ReadAllText(filePath);

                if (!string.IsNullOrWhiteSpace(pointsString))
                {
                    List<List<Point2dCanSet>> polylines = JsonConvert.DeserializeObject<List<List<Point2dCanSet>>>(pointsString);

                    List<Polyline> resultPolylines = new List<Polyline>();

                    foreach (var point2Ds in polylines)
                    {
                        Polyline singlePolyline = new Polyline();
                        for (int i = 0; i < point2Ds.Count; i++)
                        {
                            Point2d pointNew = new Point2d(point2Ds[i].X, point2Ds[i].Y);
                            singlePolyline.AddVertexAt(i, pointNew, 0, 0, 0);
                        }
                        singlePolyline.Closed = true;
                        resultPolylines.Add(singlePolyline);
                    }

                    foreach (var item in resultPolylines)
                    {
                        database.AddEntityToModelSpace(item);
                    }

                }
            }
        }


        [CommandMethod("SeeLeader")]
        public void SeeLeader()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            Database database = HostApplicationServices.WorkingDatabase;

            //1.获取输入的字符串
            PromptStringOptions psOption = new PromptStringOptions("\n 请输入点集字符串：");
            psOption.AllowSpaces = true;

            PromptResult pResult = editor.GetString(psOption);
            if (pResult.Status == PromptStatus.OK)
            {
                //2.将字符串转为点集
                string pointsString = pResult.StringResult;
                if (!string.IsNullOrWhiteSpace(pointsString))
                {
                    List<Point3dCanSet> point2Ds = JsonConvert.DeserializeObject<List<Point3dCanSet>>(pointsString);

                    Leader leader = new Leader();
                    leader.SetDatabaseDefaults();
                    foreach (var point in point2Ds)
                    {
                        leader.AppendVertex(new Point3d(point.X, point.Y, point.Z));
                    }
                    leader.HasArrowHead = true;
                    leader.ColorIndex = 3;

                    database.AddEntityToModelSpace(leader);
                }
            }
        }


    }

    public class Point2dCanSet
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class Point3dCanSet
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

}
