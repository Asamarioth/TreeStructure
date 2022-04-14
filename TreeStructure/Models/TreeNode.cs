using MySqlConnector;
using System.Data;

namespace TreeStructure.Models
{
    public class TreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }

    }
}