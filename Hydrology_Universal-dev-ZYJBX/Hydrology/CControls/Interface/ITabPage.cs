using System;

namespace Hydrology.CControls
{
    public enum ETabType { ERealTime, ERealTimeStation, EStationEdit, EErrorReport };
    public interface ITabPage
    {
        // 标题属性
        string Title { get; set; }
        // 不同的类型可以共存，同样的类型不能共存
        
        ETabType TabType { get; set; }

        // 是否需要在标签上面添加关闭按钮
        bool BTabRectClosable { get; set; }

        // 页面的索引
        int TabPageIndex { get; set; }

        // 页面已关闭
        event EventHandler TabClosed;

        // 关闭当前页面
        void CloseTab();
    }
}
