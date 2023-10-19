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
    public class EllipseExam
    {

        [CommandMethod("EllipseDemo")]
        public void EllipseDemo()
        {
            //参数一：圆心
            //参数二：平面法向量，确定XY平面
            //参数三：长轴的向量
            //参数四：短轴/长轴
            //参数五、六：起止弧度
            Ellipse ellipse = new Ellipse(new Point3d(100, 100, 0), Vector3d.ZAxis, new Vector3d(100, 0, 0), 0.4, 0, 2 * Math.PI);
            Ellipse ellipse1 = new Ellipse(new Point3d(100, 100, 0), Vector3d.ZAxis, new Vector3d(100, 0, 0), 0.6, 0, 2 * Math.PI);

            HostApplicationServices.WorkingDatabase.AddEntitiesToModelSpace(ellipse, ellipse1);
        }

        [CommandMethod("EllipseDemo1")]
        public void EllipseDemo1()
        {
            HostApplicationServices.WorkingDatabase.AppendEllipseToModelSpace(new Point3d(200,200,0),100,50,30);
        }

        [CommandMethod("EllipseDemo2")]
        public void EllipseDemo2()
        {
            HostApplicationServices.WorkingDatabase.AppendEllipseToModelSpace(new Point3d(500, 100, 0), new Point3d(300, 500, 0));
            HostApplicationServices.WorkingDatabase.AppendEllipseToModelSpace(new Point3d(100, 100, 0), new Point3d(600, 300, 0));
            HostApplicationServices.WorkingDatabase.AppendEllipseToModelSpace(new Point3d(900, 100, 0), new Point3d(700, 300, 0));
        }
    }
}
