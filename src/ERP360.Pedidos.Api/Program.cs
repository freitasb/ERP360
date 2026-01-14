using ERP360.Pedidos.Api.Validation.Pedidos;
using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Consumers;
using ERP360.Pedidos.Application.Pedidos.Commands.ConfirmarPagamento;
using ERP360.Pedidos.Application.Pedidos.Commands.CriarPedido;
using ERP360.Pedidos.Infrastructure.Messaging;
using ERP360.Pedidos.Infrastructure.Persistence;
using ERP360.Pedidos.Infrastructure.Persistence.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 👇 1) Registramos o suporte a Controllers no pipeline de DI.
// Dá pra imaginar isso como: "app, quero usar o modelo MVC/API com Controllers".
builder.Services.AddControllers();

builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PedidosDb")));


// ⚙ FluentValidation com integração automática ao pipeline da API.
builder.Services.AddFluentValidationAutoValidation(); // validação automática dos models
builder.Services.AddValidatorsFromAssemblyContaining<CriarPedidoDtoValidator>();

//Agora estamos registrando o MediatR oficialmente para a camada Application.
builder.Services.AddMediatR(typeof(CriarPedidoCommand).Assembly);
builder.Services.AddMediatR(typeof(ConfirmarPagamentoCommand).Assembly);

// Ports de saída (Application -> Infrastructure InMemory, por enquanto).
//builder.Services.AddScoped<IPedidoRepository, PedidoRepositoryInMemory>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IEstoqueReadOnlyService, EstoqueReadOnlyStub>();
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});


builder.Services.AddScoped<IPublishEvent, RabbitMqEventBus>();

var app = builder.Build();

// (No futuro entra aqui: UseAuthentication, UseAuthorization, UseCors, etc.)

// 👇 2) Dizemos para o ASP.NET Core: 
// "procure classes que herdam de ControllerBase / Controller e use as rotas delas".
app.MapControllers();

app.Run();
