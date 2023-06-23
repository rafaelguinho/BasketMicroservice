using Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Interfaces;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["CacheSettings:ConnectionString"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapGet("/api/v1/Basket/{userName}", async ([FromRoute] string userName, IBasketRepository repository) =>
{
    var basket = await repository.GetBasket(userName);
    basket ??= new ShoppingCart(userName);
    return Results.Ok(basket);
});

app.MapPost("/api/v1/Basket", async ([FromBody] ShoppingCart basket, IBasketRepository repository) =>
{
    return Results.Ok(await repository.UpdateBasket(basket));
});

app.MapDelete("/api/v1/Basket/{userName}", async ([FromRoute] string userName, IBasketRepository repository) =>
{
    return Results.Ok(await repository.DeleteBasket(userName));
});

app.Run();