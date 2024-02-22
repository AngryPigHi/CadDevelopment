using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CADTools.RibbonTools
{
    public static partial class RibbonTool
    {

        /// <summary>
        /// 为Ribbon控件添加选项卡
        /// </summary>
        /// <param name="ribbonCtrl">Ribbon控件</param>
        /// <param name="title">选项卡标题</param>
        /// <param name="ID">选项卡ID</param>
        /// <param name="isActive">是否激活该选项卡</param>
        /// <returns>新创建的选项卡</returns>
        public static RibbonTab AddTab(this RibbonControl ribbonCtrl, string title, string ID, bool isActive)
        {
            RibbonTab tab = new RibbonTab();
            tab.Title = title;
            tab.Id = ID;
            tab.IsActive = isActive;
            ribbonCtrl.Tabs.Add(tab);
            return tab;
        }

        /// <summary>
        /// 为选项卡添加面板
        /// </summary>
        /// <param name="tab">Ribbon选项卡</param>
        /// <param name="title">面板标题</param>
        /// <returns>面板数据源</returns>
        public static RibbonPanelSource AddPanel(this RibbonTab tab, string title)
        {
            RibbonPanelSource panelSource = new RibbonPanelSource();
            panelSource.Title = title;
            RibbonPanel ribbonPanel = new RibbonPanel();
            ribbonPanel.Source = panelSource;
            tab.Panels.Add(ribbonPanel);
            return panelSource;
        }



    }
}
