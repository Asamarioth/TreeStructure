using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TreeStructure.Models;

namespace TreeStructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDb _appDb;
        private TreeNodeRepository _treeRepository;

        public HomeController(AppDb db)
        {
            _appDb = db;
            _treeRepository = new TreeNodeRepository(db);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPut]
        public async Task<IActionResult> InsertNode(TreeNode node)
        {
            try
            {
                if (node.ParentId.HasValue)
            {
                if (_treeRepository.FindOneAsync(node.ParentId.Value) is null)
                {
                    return BadRequest("Parent does not exist");
                }
            }
 
                await _treeRepository.InsertOneAsync(node);
                return Ok("Node has been created");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteNode(TreeNode node)
        {
            try
            {
                if (_treeRepository.FindOneAsync(node.Id) is null)
                {
                    return BadRequest("Node not found");
                }
                await _treeRepository.RemoveNodeRecursively(node.Id);
                return Ok("Node has been deleted");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch]
        public async Task<IActionResult> MoveNode(TreeNode child, TreeNode? newParent)
        {
            try
            {
                if (_treeRepository.FindOneAsync(child.Id) is null)
            {
                return BadRequest("Node does not exist");
            }
            if(newParent is null)
            {
                await _treeRepository.ChangeParentAsync(child.Id, null);
                return Ok();

            }
            if(_treeRepository.FindOneAsync(newParent.Id) is null)
            {
                return BadRequest("Parent does not exist");
            }

               await _treeRepository.ChangeParentAsync(child.Id, newParent.Id);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateNode(TreeNode node)
        {
            try
            {
                if(_treeRepository.FindOneAsync(node.Id) is null)
                {
                    return BadRequest("Node does not exist");
                }
                await _treeRepository.UpdateNameAsync(node);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}