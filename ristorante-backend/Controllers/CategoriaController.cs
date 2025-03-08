using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ristorante_backend.Models;
using ristorante_backend.Repositories;

namespace ristorante_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriaController : ControllerBase
    {
        private CategoriaRepository _categoriaRepository;
        public CategoriaController(CategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(string? nome)
        {
            try
            {
                if (nome == null)
                {
                    return Ok(await _categoriaRepository.GetCategorie());
                }
                else
                {
                    return Ok(await _categoriaRepository.GetCategorieByNome(nome));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCategoriaById(int id)
        {
            try
            {
                Categoria categoria = await _categoriaRepository.GetCategoriaById(id);
                return categoria == null ? NotFound() : Ok(categoria);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] Categoria categoria)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState.Values);
                }
                categoria.Id = 0;
                int createdCategoriaId = await _categoriaRepository.InsertCategoria(categoria);
                return Created($"/{ControllerContext.ActionDescriptor.ControllerName}/{createdCategoriaId}", $"è stata crata una categoria con l' id: {createdCategoriaId}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Categoria categoria)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState.Values);
                }
                int affectedRows = await _categoriaRepository.UpdateCategoria(id, categoria);
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
                int affectedRows = await _categoriaRepository.DeleteCategoria(id);
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
