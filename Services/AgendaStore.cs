using System.Text.Json;
using agendaClaude.Models;

namespace agendaClaude.Services;

public sealed class AgendaStore
{
    private readonly string filePath;
    private readonly SemaphoreSlim gate = new(1, 1);
    private readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public AgendaStore(IWebHostEnvironment environment)
    {
        filePath = Path.Combine(environment.ContentRootPath, "data", "agenda.json");
    }

    public async Task<IReadOnlyList<AgendaItem>> GetAllAsync()
    {
        await gate.WaitAsync();
        try { return await ReadAsync(); }
        finally { gate.Release(); }
    }

    public async Task<AgendaItem?> GetAsync(int id)
    {
        return (await GetAllAsync()).FirstOrDefault(item => item.Id == id);
    }

    public async Task<AgendaItem> CreateAsync(AgendaItemRequest request)
    {
        await gate.WaitAsync();
        try
        {
            var items = await ReadAsync();
            var item = new AgendaItem
            {
                Id = items.Count == 0 ? 1 : items.Max(existing => existing.Id) + 1,
                Descricao = request.Descricao.Trim(),
                Data = request.Data,
                Valor = request.Valor
            };
            items.Add(item);
            await WriteAsync(items);
            return item;
        }
        finally { gate.Release(); }
    }

    public async Task<AgendaItem?> UpdateAsync(int id, AgendaItemRequest request)
    {
        await gate.WaitAsync();
        try
        {
            var items = await ReadAsync();
            var item = items.FirstOrDefault(existing => existing.Id == id);
            if (item is null) return null;
            item.Descricao = request.Descricao.Trim();
            item.Data = request.Data;
            item.Valor = request.Valor;
            await WriteAsync(items);
            return item;
        }
        finally { gate.Release(); }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await gate.WaitAsync();
        try
        {
            var items = await ReadAsync();
            var removed = items.RemoveAll(item => item.Id == id) > 0;
            if (removed) await WriteAsync(items);
            return removed;
        }
        finally { gate.Release(); }
    }

    private async Task<List<AgendaItem>> ReadAsync()
    {
        if (!File.Exists(filePath)) return [];
        await using var stream = File.OpenRead(filePath);
        var storedItems = await JsonSerializer.DeserializeAsync<List<AgendaItemStorage>>(stream, options) ?? [];
        return storedItems.Select(item => new AgendaItem
        {
            Id = item.Id,
            Descricao = item.Descricao ?? item.Nome ?? string.Empty,
            Data = item.Data,
            Valor = item.Valor
        }).ToList();
    }

    private async Task WriteAsync(List<AgendaItem> items)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, items, options);
    }

    private sealed class AgendaItemStorage
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public string? Nome { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
    }
}