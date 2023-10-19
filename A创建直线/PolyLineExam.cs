using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CADTools;
using CADTools.GraphTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A创建图形
{
    public class PolyLineExam
    {

        [CommandMethod("PolylineDemo")]
        public void PolylineDemo()
        {
            Polyline pl = new Polyline();
            //顶点
            Point2d p1 = new Point2d(100, 100);
            Point2d p2 = new Point2d(200, 200);
            Point2d p3 = new Point2d(300, 100);
            Point2d p4 = new Point2d(400, 200);

            //参数1：序号  参数2：顶点 参数3：凸度
            //参数4,5：起止宽度 0为默认
            pl.AddVertexAt(0, p1, 0, 0, 0);
            pl.AddVertexAt(1, p2, 0.5, 0, 0);
            pl.AddVertexAt(2, p3, 0, 0, 0);
            pl.AddVertexAt(3, p4, 0, 0, 0);

            pl.Closed = true;//多段线是否自动闭合
            pl.ConstantWidth = 1;//连续线宽

            Database database = HostApplicationServices.WorkingDatabase;
            database.AddEntityToModelSpace(pl);
        }

        [CommandMethod("PolylineDemo1")]
        public void PolylineDemo1()
        {
            Polyline polyline = new Polyline();

            Point2d p1 = new Point2d(100, 100);
            Point2d p2 = new Point2d(500, 100);
            Point2d p3 = new Point2d(500, 300);
            Point2d p4 = new Point2d(100, 300);

            polyline.AddVertexAt(0, p1, 0, 0, 0);
            polyline.AddVertexAt(1, p2, 1, 0, 0);
            polyline.AddVertexAt(2, p3, 0, 0, 0);
            polyline.AddVertexAt(3, p4, 1, 0, 0);

            polyline.Closed = true;

            Database database = HostApplicationServices.WorkingDatabase;
            database.AddEntityToModelSpace(polyline);
        }


        [CommandMethod("RecDemo")]
        public void PolylineDemo2()
        {
            Point2d p1 = new Point2d(200, 200);
            Point2d p2 = new Point2d(600, 400);

            Database database = HostApplicationServices.WorkingDatabase;
            database.AppendRectAngleToModelSpace(p1, p2);
        }


        [CommandMethod("RegularPolygonDemo")]
        public void RegularPolygon()
        {
            Database database = HostApplicationServices.WorkingDatabase;

            database.AppendRegularPolygonToModelSpace(new Point2d(100,100),50,3,90);
            database.AppendRegularPolygonToModelSpace(new Point2d(200,100),50,4,45);
            database.AppendRegularPolygonToModelSpace(new Point2d(300,100),50,5,90);
            database.AppendRegularPolygonToModelSpace(new Point2d(400,100),50,6,0);
            database.AppendRegularPolygonToModelSpace(new Point2d(500,100),50,12,0);
        }


    }
}
