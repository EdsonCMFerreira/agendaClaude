using agendaClaude.Data;
using agendaClaude.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace agendaClaude.Controllers;

[ApiController]
[Route("api/agenda")]
public sealed class AgendaController(AgendaDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AgendaItem>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await db.AgendaItems.OrderByDescending(item => item.Data).ThenByDescending(item => item.Id).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AgendaItem>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await db.AgendaItems.FindAsync([id], cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<AgendaItem>> Create(AgendaItemRequest request, CancellationToken cancellationToken)
    {
        if (!IsValid(request)) return BadRequest(new { message = "Informe descrição, data e um valor válido." });
        var item = new AgendaItem { Descricao = request.Descricao.Trim(), Data = request.Data, Valor = request.Valor };
        db.AgendaItems.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AgendaItem>> Update(int id, AgendaItemRequest request, CancellationToken cancellationToken)
    {
        if (!IsValid(request)) return BadRequest(new { message = "Informe descrição, data e um valor válido." });
        var item = await db.AgendaItems.FindAsync([id], cancellationToken);
        if (item is null) return NotFound();
        item.Descricao = request.Descricao.Trim();
        item.Data = request.Data;
        item.Valor = request.Valor;
        await db.SaveChangesAsync(cancellationToken);
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var item = await db.AgendaItems.FindAsync([id], cancellationToken);
        if (item is null) return NotFound();
        db.AgendaItems.Remove(item);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static bool IsValid(AgendaItemRequest request) =>
        !string.IsNullOrWhiteSpace(request.Descricao) && request.Data != default && request.Valor >= 0;
}

public sealed class AgendaPageController : Controller
{
    [HttpGet("agenda")]
    public IActionResult Index() => View();
}
