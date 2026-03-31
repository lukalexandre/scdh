using Microsoft.EntityFrameworkCore;
using SCDH.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=habitacao.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("SegurancaCaixa", policy =>
    {
        policy.WithOrigins("http://localhost:5100")
              .WithMethods("GET", "POST")
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var horario = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    var metodo = context.Request.Method;
    var urlAcessada = context.Request.Path;
    var ipCliente = context.Connection.RemoteIpAddress?.ToString() ?? "IP Desconhecido";

    Console.WriteLine($"[SCDH AUDITORIA] {horario} | IP: {ipCliente} | Método: {metodo} | URL: {urlAcessada}");

    await next.Invoke();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("SegurancaCaixa");
app.UseAuthorization();
app.MapControllers();

app.Urls.Add("http://0.0.0.0:5286");

app.Run();