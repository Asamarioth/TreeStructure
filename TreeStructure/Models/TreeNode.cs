using MySqlConnector;
using System.Data;

namespace TreeStructure.Models
{
    public class TreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }

        internal AppDb Db { get; set; }

        public TreeNode()
        {
        }

        internal TreeNode(AppDb db)
        {
            Db = db;
        }

        public async Task InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `tree` (`Name`, `ParentId`) VALUES (@name, @parentId);";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();
            Id = (int)cmd.LastInsertedId;
        }

        public async Task UpdateNameAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `tree` SET `Name` = @name WHERE `Id` = @id;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `tree` WHERE `Id` = @id;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@id", Id);
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@name", Name);
            cmd.Parameters.AddWithValue("@parentId", ParentId);


        }
    }
}