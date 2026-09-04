namespace agendaClaude.Models;

public sealed class AgendaItem
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
}

public sealed record AgendaItemRequest(string Descricao, DateTime Data, decimal Valor);
