namespace TreeStructure.Models
{
    public class TreeView
    {
        public IEnumerable<TreeNode> GetTreeView { get; set; }
        public TreeView()
        {
            this.GetTreeView = new List<TreeNode>();
        }
    }
}
