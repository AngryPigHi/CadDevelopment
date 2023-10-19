using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADTools.PromptTools
{
    public static class PromptTools
    {
        /// <summary>
        /// 获取点或者关键字符
        /// </summary>
        /// <param name="editor">编辑命令行</param>
        /// <param name="promptText">提示信息</param>
        /// <param name="prePoint">基准点</param>
        /// <param name="keyWords">关键字符数组</param>
        /// <returns>PromptPointResult</returns>
        public static PromptPointResult GetPoint(this Editor editor, string promptText, Point3d prePoint, params string[] keyWords)
        {
            PromptPointOptions ppOption = new PromptPointOptions(promptText);
            ppOption.AllowNone = true;
            //设置基准点
            ppOption.BasePoint = prePoint;
            ppOption.UseBasePoint = true;

            //注册字符，使得相应的字符按键有效
            for (int i = 0; i < keyWords.Length; i++)
            {
                ppOption.Keywords.Add(keyWords[i]);
            }
            //取消系统的自动关键词显示
            ppOption.AppendKeywordsToMessage = false;

            return editor.GetPoint(ppOption);
        }
    }
}
