using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 👇 1) Registramos o suporte a Controllers no pipeline de DI.
// Dá pra imaginar isso como: "app, quero usar o modelo MVC/API com Controllers".
builder.Services.AddControllers();

var app = builder.Build();

// (No futuro entra aqui: UseAuthentication, UseAuthorization, UseCors, etc.)

// 👇 2) Dizemos para o ASP.NET Core: 
// "procure classes que herdam de ControllerBase / Controller e use as rotas delas".
app.MapControllers();

app.Run();
