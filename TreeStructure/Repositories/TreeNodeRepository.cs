using System.Data.Common;
using TreeStructure.Models;

namespace TreeStructure
{
    public class TreeNodeRepository
    {
        public AppDb Db { get; }

        public TreeNodeRepository(AppDb db)
        {
            Db = db;
            Db.Connection.Open();
        }

        public void InsertOne(TreeNode node)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `tree` (`Name`, `ParentId`) VALUES (@name, @parentId);";
            cmd.Parameters.AddWithValue("@name", node.Name);
            cmd.Parameters.AddWithValue("@parentId", node.ParentId);
            cmd.ExecuteNonQuery();
        }

        public TreeNode? FindOne(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `Id`, `ParentId`, `Name` FROM `tree` WHERE `Id` = @id";
            cmd.Parameters.AddWithValue("@id", id);
            var result = ReadAll(cmd.ExecuteReader());
            return result.Count > 0 ? result[0] : null;
        }

        public TreeView GetAll()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM `tree` ";
            var result = ReadAll(cmd.ExecuteReader());
            TreeView tree = new()
            {
                GetTreeView = result
            };
            return tree;
        }

        public List<TreeNode>? GetChildren(TreeNode node)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `Id`, `ParentId`, `Name` FROM `tree` WHERE `ParentId` = @parentId";
            cmd.Parameters.AddWithValue("@parentId", node.Id);
            var result = ReadAll(cmd.ExecuteReader());
            return result.Count > 0 ? result : null;
        }

        public void RemoveChildren(TreeNode node)
        {
            var children = GetChildren(node);

            if (children is not null)
            {
                foreach (var child in children)
                {
                    RemoveNodeRecursively(child);
                }
            }
        }
        public void RemoveNodeRecursively(TreeNode node)
        {
            var children = GetChildren(node);

            if (children is not null)
            {
                foreach (var child in children)
                {
                    RemoveNodeRecursively(child);
                }
            }
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `tree` WHERE `Id` = @id;";
            cmd.Parameters.AddWithValue("@id", node.Id);
            cmd.ExecuteNonQuery();
        }


        public void ChangeParent(TreeNode child, TreeNode newParent)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `tree` SET `ParentId` = @parentId WHERE `Id` = @id;";
            cmd.Parameters.AddWithValue("@parentId", newParent.Id);
            cmd.Parameters.AddWithValue("@id", child.Id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateName(TreeNode node)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `tree` SET `Name` = @name WHERE `Id` = @id;";
            cmd.Parameters.AddWithValue("@name", node.Name);
            cmd.Parameters.AddWithValue("@id", node.Id);
            cmd.ExecuteNonQuery();
        }

        public List<TreeNode> ReadAll(DbDataReader reader)
        {
            var nodes = new List<TreeNode>();
            using (reader)
            {
                while (reader.Read())
                {
                    var node = new TreeNode()
                    {
                        Id = reader.GetInt32(0),
                        ParentId = reader.GetInt32(1),
                        Name = reader.GetString(2),
                    };
                    nodes.Add(node);
                }
            }
            return nodes;
        }
    }
}