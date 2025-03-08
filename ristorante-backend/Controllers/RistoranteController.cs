using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ristorante_backend.Models;
using ristorante_backend.Repositories;

namespace ristorante_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RistoranteController : ControllerBase
    {
        private RistoranteRepository _ristoranteRepository;
        public RistoranteController(RistoranteRepository ristoranteRepository)
        {
            _ristoranteRepository = ristoranteRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(string? nome)
        {
            try
            {
                if (nome == null)
                {
                    return Ok(await _ristoranteRepository.GetRistoranti());
                }
                else
                {
                    return Ok(await _ristoranteRepository.GetRistoranteByNome(nome));
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
                Ristorante ristorante = await _ristoranteRepository.GetRistoranteById(id);
                return ristorante == null ? NotFound() : Ok(ristorante);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] Ristorante ristorante)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState.Values);
                }
                ristorante.Id = 0;
                int createdRistoranteId = await _ristoranteRepository.InsertRistorante(ristorante);
                return Created($"/{ControllerContext.ActionDescriptor.ControllerName}/{createdRistoranteId}", $"è stato crato una ristorante con l' id: {createdRistoranteId}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Ristorante ristorante)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState.Values);
                }
                int affectedRows = await _ristoranteRepository.UpdateRistorante(id, ristorante);
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
                int affectedRows = await _ristoranteRepository.DeleteRistorante(id);
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
