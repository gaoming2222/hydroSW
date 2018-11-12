
namespace Hydrology.CControls
{
    /// <summary>
    /// 树形控件节点类型
    /// </summary>
    public enum CTreeType
    {
        SystemSetting,  //系统管理
        ChannelProtocolCfg,//通讯方式配置
        ChannelProtocolCfg2,//通讯方式配置二级菜单
        DataProtocolCfg,//数据协议配置
        DataProtocolCfg2,//数据协议配置二级菜单
        CommunicationPort,//通讯口配置
        CommunicationPort2,//通讯口配置二级菜单
        DatabaseConfig,//数据库配置
        DatabaseConfig2,//数据库配置二级菜单
        VoiceConfig,//声音配置
        VoiceConfig2,//声音配置二级菜单
        //PortSettings,   //串口配置
        //PortSettings2,  //串口配置二级菜单
        //ProtocolType,   //数据协议
        //ProtocolType2,  //数据协议二级菜单
        // MessageType,    //信道协议
        //MessageType2,     //信道协议二级菜单
        UserSetting,    //用户管理
        UserLogin,//用户二级登陆菜单
        UserLogin2,//用户二级登陆菜单
        CeZhan1,        //遥测站设置一级目录
        CeZhan2,        //遥测站设置二级目录
        CeZhan3,        //遥测站设置三级目录
        CeZhan4         //遥测站设置四级目录
    }
}
