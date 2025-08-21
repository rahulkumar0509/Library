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

app.UseHttpsRedirection();
app.MapControllers();
app.MapLibraryEndpoint();

// middleware is singleton context, so define all the Scoped services in invoke mthod instead of constructor.
// app.UseLibraryAuthenticationMiddleware();
// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();
app.Run();