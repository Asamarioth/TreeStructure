using MySqlConnector;
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
        }
        public async Task InsertOneAsync(TreeNode node)
        {
           await node.InsertAsync();
        }
        public async Task<TreeNode?> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `Id`, `ParentId`, `Name` FROM `tree` WHERE `Id` = @id";
            cmd.Parameters.AddWithValue("@id", id);
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<List<TreeNode>?> GetChildrenAsync(int parentId)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `Id`, `ParentId`, `Name` FROM `tree` WHERE `ParentId` = @parentId";
            cmd.Parameters.AddWithValue("@parentId", parentId);
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result : null;
        }
        public async Task RemoveNodeRecursively(int id)
        {
            var children = await GetChildrenAsync(id);

            if (children is not null)
            {
                foreach (var child in children)
                {
                    await RemoveNodeRecursively(child.Id);
                }
            }
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `tree` WHERE `Id` = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }
        public async Task ChangeParentAsync(int id, int? newParentId)
        {
            using var cmd = Db.Connection.CreateCommand();
            if (newParentId.HasValue)
            {
                cmd.CommandText = @"UPDATE `tree` SET `ParentId` = @parentId WHERE `Id` = @id;";
                cmd.Parameters.AddWithValue("@parentId", newParentId);
                cmd.Parameters.AddWithValue("@id", id);
            }
            else
            {
                cmd.CommandText = @"UPDATE `tree` SET `ParentId` = NULL WHERE `Id` = @id;";
                cmd.Parameters.AddWithValue("@id", id);
            }
            await cmd.ExecuteNonQueryAsync();

        }
        public async Task UpdateNameAsync(TreeNode node)
        {
            await node.UpdateNameAsync();
        }
        public async Task<List<TreeNode>> ReadAllAsync(DbDataReader reader)
        {
            var nodes = new List<TreeNode>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var node = new TreeNode(Db)
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