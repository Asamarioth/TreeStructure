using Microsoft.AspNetCore.Mvc;
using TreeStructure.Models;

namespace TreeStructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly TreeNodeRepository _treeRepository;

        public HomeController(AppDb db)
        {
            _treeRepository = new TreeNodeRepository(db);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult<List<TreeNode>?> GetChildren(int id)
        {
            try
            {
                return _treeRepository.GetChildren(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public ActionResult<List<TreeNode>> GetAll()
        {
            try
            {
                return _treeRepository.GetAll().GetTreeView.ToList();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult InsertNode([FromBody]TreeNode node)
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
        public IActionResult DeleteNode(int id)
        {
            try
            {
                if(id == 0)
                {
                    _treeRepository.RemoveChildren(id);
                    return Ok("Tree cleared");
                }
                if (_treeRepository.FindOne(id) is null)
                {
                    return BadRequest("Node not found");
                }
                _treeRepository.RemoveNodeRecursively(id);
                return Ok("Node has been deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch]
        public IActionResult UpdateNode([FromBody]TreeNode node)
        {
            Console.WriteLine(node);
            try
            {
                if (_treeRepository.FindOne(node.Id) is null)
                {
                    return BadRequest("Node does not exist");
                }
                if (node.ParentId != 0)
                {
                    if (_treeRepository.FindOne(node.ParentId) is null)
                    {
                        return BadRequest("Parent does not exist");
                    }
                }
                _treeRepository.UpdateNode(node);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}