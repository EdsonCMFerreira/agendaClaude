namespace agendaClaude.Models;

public sealed class AgendaItem
{
    public int Id { get; set; }
    public string Contato { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public TimeSpan Horario { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

public sealed record AgendaItemRequest(string Contato, DateTime Data, TimeSpan Horario, string Telefone, decimal Valor);
