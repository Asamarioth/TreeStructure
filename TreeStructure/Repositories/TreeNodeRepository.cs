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

        public List<TreeNode> GetAll()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM `tree` ORDER BY `ParentId`";
            var result = ReadAll(cmd.ExecuteReader());

            return result;
        }

        public List<TreeNode>? GetChildren(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `Id`, `ParentId`, `Name` FROM `tree` WHERE `ParentId` = @parentId";
            cmd.Parameters.AddWithValue("@parentId", id);
            var result = ReadAll(cmd.ExecuteReader());
            return result.Count > 0 ? result : null;
        }

        public void RemoveChildren(int id)
        {
            var children = GetChildren(id);

            if (children is not null)
            {
                foreach (var child in children)
                {
                    RemoveNodeRecursively(child.Id);
                }
            }
        }
        public void RemoveNodeRecursively(int id)
        {
            var children = GetChildren(id);

            if (children is not null)
            {
                foreach (var child in children)
                {
                    RemoveNodeRecursively(child.Id);
                }
            }
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `tree` WHERE `Id` = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateNode(TreeNode node)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `tree` SET `Name` = @name, `ParentId` = @parentId WHERE `Id` = @id;";
            cmd.Parameters.AddWithValue("@name", node.Name);
            cmd.Parameters.AddWithValue("@parentId", node.ParentId);
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