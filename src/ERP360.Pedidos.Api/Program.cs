using ERP360.Pedidos.Api.Validaion.Pedidos;
using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Pedidos.Commands.CriarPedido;
using ERP360.Pedidos.Infrastructure.InMemory;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 👇 1) Registramos o suporte a Controllers no pipeline de DI.
// Dá pra imaginar isso como: "app, quero usar o modelo MVC/API com Controllers".
builder.Services.AddControllers();

// ⚙ FluentValidation com integração automática ao pipeline da API.
builder.Services.AddFluentValidationAutoValidation(); // validação automática dos models
builder.Services.AddValidatorsFromAssemblyContaining<CriarPedidoDtoValidator>();

//Agora estamos registrando o MediatR oficialmente para a camada Application.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CriarPedidoCommand).Assembly));

// Ports de saída (Application -> Infrastructure InMemory, por enquanto).
builder.Services.AddScoped<IPedidoRepository, PedidoRepositoryInMemory>();
builder.Services.AddScoped<IEstoqueReadOnlyService, EstoqueReadOnlyStub>();
builder.Services.AddScoped<IPublishEvent, EventCollector>();

var app = builder.Build();

// (No futuro entra aqui: UseAuthentication, UseAuthorization, UseCors, etc.)

// 👇 2) Dizemos para o ASP.NET Core: 
// "procure classes que herdam de ControllerBase / Controller e use as rotas delas".
app.MapControllers();

app.Run();
