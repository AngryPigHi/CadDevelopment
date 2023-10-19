using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools.JigTools
{
    public class CircleJig : EntityJig
    {
        public double JRadius { get; set; }

        public CircleJig(Point3d centerPoint) : base(new Circle())
        {
            //Entity是父类中的属性
            (this.Entity as Circle).Center = centerPoint;
        }


        //这个函数的作用是鼠标在屏幕上移动时就会调用
        //实现这个函数一般是用它改变图形的属性（我们在这个类中定义的属性）
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions jigPromptPointOptions = new JigPromptPointOptions("\n 请指定圆上的一个点：");
            jigPromptPointOptions.BasePoint = (this.Entity as Circle).Center;
            jigPromptPointOptions.UseBasePoint = true;
            jigPromptPointOptions.Cursor = CursorType.RubberBand;
            jigPromptPointOptions.AppendKeywordsToMessage = false;

            PromptPointResult ppResult = prompts.AcquirePoint(jigPromptPointOptions);

            if (ppResult.Status == PromptStatus.OK)
            {
                this.JRadius = ppResult.Value.GetDistanceBetweenTwoPoints((this.Entity as Circle).Center);
            }

            return SamplerStatus.NoChange;
        }

        //用于更新图形对象，这个更新属性是不需要通过事务来处理的
        protected override bool Update()
        {
            //动态更新圆的半径
            if (JRadius > 0)
            {
                (this.Entity as Circle).Radius = JRadius;
            }
            return true;
        }

        /// <summary>
        /// 获取圆图形
        /// </summary>
        /// <returns></returns>
        public Circle GetCircle()
        {
            return (Circle)Entity;
        }
    }
}
