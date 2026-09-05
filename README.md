# agendaClaude

## Descrição

Sua Agenda.Net é uma aplicação web MVC para organizar contatos e compromissos de forma simples e centralizada. Ela permite cadastrar, consultar, editar e excluir registros com nome de contato, data, horário, telefone e valor. A aplicação foi desenvolvida em ASP.NET Core, usa Entity Framework Core e persiste os dados em SQL Server.

## Campos

- Contato
- Data
- Horário
- Telefone
- Valor

## Executar

```powershell
dotnet run --urls http://localhost:5001
```

Acesse `http://localhost:5001` no navegador.

O banco `AgendaClaude` e a tabela `AgendaItems` são criados automaticamente pelo EF Core na inicialização.

## API

- `GET /api/agenda` lista os compromissos.
- `GET /api/agenda/{id}` consulta um compromisso.
- `POST /api/agenda` cria um compromisso.
- `PUT /api/agenda/{id}` atualiza um compromisso.
- `DELETE /api/agenda/{id}` exclui um compromisso.

A conexão padrão usa a instância local `SQLEXPRESS`:

```text
Server=.\SQLEXPRESS;Database=AgendaClaude;Trusted_Connection=True;TrustServerCertificate=True;
```
