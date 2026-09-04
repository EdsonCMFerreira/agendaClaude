namespace agendaClaude.Models;

public sealed class AgendaItem
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
}

public sealed record AgendaItemRequest(string Nome, DateTime Data, decimal Valor);
