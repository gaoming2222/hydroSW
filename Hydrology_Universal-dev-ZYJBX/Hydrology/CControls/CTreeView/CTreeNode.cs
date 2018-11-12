
namespace Hydrology.CControls
{
    /// <summary>
    /// 自定义树形节点
    /// </summary>
    public class CTreeNode : System.Windows.Forms.TreeNode
    {
        public CTreeNode(string text, CTreeType type, string id = "0")
            : base(text)
        {
            this.type = type;
            this.m_sid = id;
        }

        public void SetImageIndex(int index)
        {
            this.ImageIndex = index;
            this.SelectedImageIndex = index + 1;
        }

        private CTreeType type;
        public CTreeType Type
        {
            get { return this.type; }
        }
        private string m_sid;
        public string ID
        {
            get { return this.m_sid; }
        }
    }
}
