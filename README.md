# agendaClaude

## Descrição

Sua Agenda.Net é uma aplicação web para organizar compromissos de forma simples e centralizada. Ela permite cadastrar, consultar, editar e excluir compromissos, acompanhando a descrição, a data e o valor de cada registro. A aplicação foi desenvolvida em ASP.NET Core e utiliza um arquivo JSON local para persistir os dados, sem necessidade de configurar um banco de dados.

## Campos

- Descrição
- Data
- Valor

## Executar

```powershell
dotnet run --urls http://localhost:5001
```

Acesse `http://localhost:5001` no navegador.

Os dados são persistidos em `data/agenda.json`.
