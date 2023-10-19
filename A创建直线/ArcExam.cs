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
    public class ArcExam
    {

        [CommandMethod("ArcDemo")]
        public void ArcDemo()
        {
            Arc arc1 = new Arc();
            arc1.Center = new Point3d(100, -100, 0);
            arc1.Radius = 150;

            arc1.StartAngle = 10.0.DegreeToAngle();
            arc1.EndAngle = 130.0.DegreeToAngle();

            Arc arc2 = new Arc(new Point3d(0, 0, 0), 100, Math.PI / 4, Math.PI * 3 / 2);
            Arc arc3 = new Arc(new Point3d(120, 120, 0), 80, 20.0.DegreeToAngle(), 80.0.DegreeToAngle());

            Database db = HostApplicationServices.WorkingDatabase;
            db.AddEntitiesToModelSpace(arc1, arc2, arc3);
        }

        [CommandMethod("ArcDemo1")]
        public void ArcDemo1()
        {
            //三点画圆弧
            Point3d startPoint = new Point3d(100, 100, 0);
            Point3d onArcPoint = new Point3d(150, 100, 0);
            Point3d endPoint = new Point3d(80, 200, 0);
            CircularArc3d circularArc3D = new CircularArc3d(startPoint, onArcPoint, endPoint);

            //将三点画的圆弧转为Arc对象
            Point3d centerPoint = circularArc3D.Center;
            double radius = circularArc3D.Radius;

            Vector3d c2s = centerPoint.GetVectorTo(startPoint);
            Vector3d c2e = centerPoint.GetVectorTo(endPoint);
            Vector3d xVector = new Vector3d(1, 0, 0);

            //根据圆心到端点的向量，判断弧度是正负
            //弧度一般在[-π,π]之间，也就是[-180°,180°]
            double startRadian = c2s.Y > 0 ? xVector.GetAngleTo(c2s) : -xVector.GetAngleTo(c2s);
            double endRadian = c2e.Y > 0 ? xVector.GetAngleTo(c2e) : -xVector.GetAngleTo(c2e);

            Arc arc = new Arc(centerPoint, radius, startRadian, endRadian);

            Database database = HostApplicationServices.WorkingDatabase;
            database.AddEntityToModelSpace(arc);
        }


        [CommandMethod("ArcDemo2")]
        public void ArcDemo2()
        {
            Database database = HostApplicationServices.WorkingDatabase;
            database.AppendArcToModelSpace(new Point3d(100, 100, 0), new Point3d(150, 100, 0), new Point3d(200, 200, 0));
        }


        [CommandMethod("ArcDemo3")]
        public void ArcDemo3()
        {
            Database database = HostApplicationServices.WorkingDatabase;
            database.AppendArcToModelSpace(new Point3d(0,0,0),new Point3d(100,100,0),-30);
        }

    }
}
