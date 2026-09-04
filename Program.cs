using agendaClaude.Models;
using agendaClaude.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AgendaStore>();
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/agenda", async (AgendaStore store) => Results.Ok(await store.GetAllAsync()));

app.MapGet("/api/agenda/{id:int}", async (int id, AgendaStore store) =>
{
	var item = await store.GetAsync(id);
	return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/api/agenda", async (AgendaItemRequest request, AgendaStore store) =>
{
	if (string.IsNullOrWhiteSpace(request.Descricao) || request.Data == default || request.Valor < 0)
		return Results.BadRequest(new { message = "Informe descrição, data e um valor válido." });

	var item = await store.CreateAsync(request);
	return Results.Created($"/api/agenda/{item.Id}", item);
});

app.MapPut("/api/agenda/{id:int}", async (int id, AgendaItemRequest request, AgendaStore store) =>
{
	if (string.IsNullOrWhiteSpace(request.Descricao) || request.Data == default || request.Valor < 0)
		return Results.BadRequest(new { message = "Informe descrição, data e um valor válido." });

	var item = await store.UpdateAsync(id, request);
	return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapDelete("/api/agenda/{id:int}", async (int id, AgendaStore store) =>
	await store.DeleteAsync(id) ? Results.NoContent() : Results.NotFound());

app.Run();
