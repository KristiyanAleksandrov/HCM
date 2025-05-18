using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using People.API.Infrastructure.ErrorHandling;
using People.API.Infrastructure.Vault;
using People.Application.Interfaces;
using People.Application.Services;
using People.Infrastructure.Data;
using People.Infrastructure.Interceptors;
using People.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddSingleton<SoftDeleteInterceptor>();

var vault = new VaultService();

//get secrets from Vault
var dbSecrets = await vault.GetSecretAsync("hcm/db");
builder.Configuration["ConnectionStrings:PeopleDb"] = dbSecrets["PeopleDb"]?.ToString();

builder.Services.AddDbContext<PeopleDbContext>((sp, opts) =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("PeopleDb"))
    .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>()));

//get secrets from Vault
var jwtSecrets = await vault.GetSecretAsync("hcm/jwt");
builder.Configuration["Jwt:Secret"] = jwtSecrets["Secret"]?.ToString();
builder.Configuration["Jwt:Issuer"] = jwtSecrets["Issuer"]?.ToString();
builder.Configuration["Jwt:Audience"] = jwtSecrets["Audience"]?.ToString();
builder.Configuration["Jwt:ExpiryMinutes"] = jwtSecrets["ExpiryMinutes"]?.ToString();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Secret"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

//TODO: Make shared projects for the repeated code.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<PeopleDbContext>();
        db.Database.Migrate();
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
