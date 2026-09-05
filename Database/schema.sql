create database AgendaClaude;
go

use AgendaClaude;
go

create table AgendaItems (
    Id int identity(1,1) primary key,
    Contato nvarchar(120) not null,
    Data date not null,
    Horario time not null,
    Telefone nvarchar(20) not null,
    Valor decimal(12,2) not null constraint CK_AgendaItems_Valor check (Valor >= 0)
);
go
