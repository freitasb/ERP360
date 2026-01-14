using ERP360.Estoque.Api.Infrastructure;
using ERP360.Estoque.Api.Messaging.Consumers;
using ERP360.Estoque.Application.Abstractions;
using ERP360.Estoque.Application.Reservas.Command.ReservarEstoqueDoPedido;
using ERP360.Estoque.Infrastructure.Persistence.InMemory;
using MassTransit;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMediatR(typeof(ReservarEstoqueDoPedidoCommand).Assembly);

builder.Services.AddScoped<IEstoqueRepository, EstoqueRepositoryInMemory>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PedidoPagoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("erp360.pedidos.pedido-pago", e =>
        {
            e.ConfigureConsumer<PedidoPagoConsumer>(context);
        });
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
