using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ristorante_backend.Models;
using ristorante_backend.Repositories;

namespace ristorante_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class MenuController : Controller
    {
        private readonly MenuRepository _menuRepository;

        public MenuController(MenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        [HttpGet]
        [Authorize]

        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _menuRepository.GetAllMenuAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]

        public async Task<IActionResult> GetMenuById(int id)
        {
            try
            {
                Menu m = await _menuRepository.GetMenuByIdAsync(id);
                if (m == null)
                {
                    return NotFound();
                }
                return Ok(m);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{id}/Piatti")]
        [Authorize]

        public async Task<IActionResult> GetMenuPiattiById(int id)
        {
            try
            {
                List<Piatto> piatti = await _menuRepository.GetAllPiattoFromMenuId(id);
                if (piatti == null)
                {
                    return NotFound();
                }
                return Ok(piatti);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("Piatti")]
        [Authorize]

        public async Task<IActionResult> GetMenuPiattiByNome(string nome)
        {
            try
            {
                List<Piatto> piatti = await _menuRepository.GetAllPiattoFromMenuNome(nome);
                if (piatti == null)
                {
                    return NotFound();
                }
                return Ok(piatti);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Create([FromBody] Menu menu)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState.Values);
                }
                menu.Id = 0;
                int menuId = await _menuRepository.InsertMenu(menu);
                return Created($"/{ControllerContext.ActionDescriptor.ControllerName}/{menuId}", $"è stato creato un menù con l' id: {menuId} e il nome: {menu.Nome}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Update(int id, [FromBody] Menu menu)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState.Values);
                }
                int affectedRows = await _menuRepository.UpdateMenuAsync(id, menu);
                if (affectedRows == 0)
                {
                    return NotFound();
                }
                return Ok(affectedRows);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int affectedRows = await _menuRepository.DeleteMenuAsync(id);
                if (affectedRows == 0)
                {
                    return NotFound();
                }
                return Ok(affectedRows);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
