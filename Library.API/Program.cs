using Library.Services.Impl;
using Library.Services;
using Library.Repository.Impl;
using Library.Repository;
using Library.Domain;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Library.API.Endpoints;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// service registration
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

// Controller registration
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapLibraryEndpoint();

app.Run();