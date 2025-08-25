using Library.Services.Impl;
using Library.Services;
using Library.Repository.Impl;
using Library.Repository;
using Library.API.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Library.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// service registration
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookAuthorsRepository, BookAuthorsRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Controller registration
builder.Services.AddControllers();

builder.Services.AddProblemDetails(); // It works as fallback plan for UseExceptionHandler so it is mandatory. When you call builder.Services.AddProblemDetails(), the framework adds its own default IExceptionHandler implementation to the dependency injection container. This handler is a built-in component of the ASP.NET Core framework designed to automatically generate RFC 9457-compliant error responses. You don't need to manually create or configure these services; they are part of the framework's internal plumbing for standardized error handling.
builder.Services.AddExceptionHandler<LibraryExceptionHandler>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// JWT Authentication
// It doesn’t validate tokens at this point — it just sets up the rules so that when a request comes in later,
// ASP.NET Core knows how to handle JWT tokens.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Inline middleware
// app.Use(async (context, next) =>
// {
//     var body = context.Request.Body;
//     var header = context.Request.Headers;
//     Console.WriteLine(body);
//     Console.WriteLine(JsonSerializer.Serialize(header).ToString());
//     await next(context);
// });
app.UseExceptionHandler(); // Must add .AddProblemDetails()
app.UseHttpsRedirection();
app.MapControllers();
app.MapLibraryEndpoint();


// middleware is singleton context, so define all the Scoped services in invoke mthod instead of constructor.
app.UseLibraryAuthenticationMiddleware();
app.UseLibraryExceptionHandlerMiddleware();
// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();
app.Run();