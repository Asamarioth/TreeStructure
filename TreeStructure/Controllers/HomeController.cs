using Microsoft.AspNetCore.Mvc;
using TreeStructure.Models;

namespace TreeStructure.Controllers
{
    public class HomeController : Controller
    {
        // private readonly AppDb _appDb;
        private readonly TreeNodeRepository _treeRepository;

        public HomeController(AppDb db)
        {
            //_appDb = db;
            _treeRepository = new TreeNodeRepository(db);
        }

        public ActionResult Index()
        {
            TreeNode node = new()
            {
                Id = 10
            };
            //MoveNode(node, new TreeNode { Id = 9});
            return View(_treeRepository.GetAll());
        }

        [HttpGet]
        public ActionResult<TreeView> GetTree()
        {
            try
            {
                return _treeRepository.GetAll();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult InsertNode(TreeNode node)
        {
            try
            {
                if (node.ParentId != 0)
                {
                    if (_treeRepository.FindOne(node.ParentId) is null)
                    {
                        return BadRequest("Parent does not exist");
                    }
                }
                _treeRepository.InsertOne(node);
                return Ok("Node has been created");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteNode(TreeNode node)
        {
            try
            {
                if(node.Id == 0)
                {
                    _treeRepository.RemoveChildren(node);
                }
                if (_treeRepository.FindOne(node.Id) is null)
                {
                    return BadRequest("Node not found");
                }
                _treeRepository.RemoveNodeRecursively(node);
                return Ok("Node has been deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        public IActionResult MoveNode(TreeNode child, TreeNode? newParent)
        {
            try
            {
                if (_treeRepository.FindOne(child.Id) is null)
                {
                    return BadRequest("Node does not exist");
                }
                if (newParent is null)
                {
                    _treeRepository.ChangeParent(child, new TreeNode { Id = 0 });
                    return Ok();
                }
                if (_treeRepository.FindOne(newParent.Id) is null)
                {
                    return BadRequest("Parent does not exist");
                }

                _treeRepository.ChangeParent(child, newParent);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        public IActionResult UpdateNode(TreeNode node)
        {
            try
            {
                if (_treeRepository.FindOne(node.Id) is null)
                {
                    return BadRequest("Node does not exist");
                }
                _treeRepository.UpdateName(node);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}