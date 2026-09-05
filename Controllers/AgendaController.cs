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
        if (!IsValid(request)) return BadRequest(new { message = "Informe contato, data, horário, telefone e um valor válido." });
        var item = new AgendaItem
        {
            Contato = request.Contato.Trim(),
            Data = request.Data,
            Horario = request.Horario,
            Telefone = request.Telefone.Trim(),
            Valor = request.Valor
        };
        db.AgendaItems.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AgendaItem>> Update(int id, AgendaItemRequest request, CancellationToken cancellationToken)
    {
        if (!IsValid(request)) return BadRequest(new { message = "Informe contato, data, horário, telefone e um valor válido." });
        var item = await db.AgendaItems.FindAsync([id], cancellationToken);
        if (item is null) return NotFound();
        item.Contato = request.Contato.Trim();
        item.Data = request.Data;
        item.Horario = request.Horario;
        item.Telefone = request.Telefone.Trim();
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
        !string.IsNullOrWhiteSpace(request.Contato) &&
        !string.IsNullOrWhiteSpace(request.Telefone) &&
        request.Data != default &&
        request.Horario >= TimeSpan.Zero &&
        request.Valor >= 0;
}

public sealed class AgendaPageController : Controller
{
    [HttpGet("agenda")]
    public IActionResult Index() => View();
}
