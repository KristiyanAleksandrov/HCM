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
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IPeopleRepository, PeopleRepository>();
builder.Services.AddScoped<IPeopleService, PeopleService>();
builder.Services.AddSingleton<SoftDeleteInterceptor>();

var myAllowSpecificOrigins = "myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

if (!builder.Environment.IsEnvironment("IntegrationTests"))
{
    var vaultUri = builder.Configuration["Vault:Uri"];
    var vaultToken = builder.Configuration["Vault:Token"];
    var vault = new VaultSecretProvider(vaultUri, vaultToken);

    var dbSecrets = await vault.GetSecretAsync("hcm/db");
    builder.Configuration["ConnectionStrings:PeopleDb"] = dbSecrets["PeopleDb"]?.ToString();

    builder.Services.AddDbContext<PeopleDbContext>((sp, opts) =>
        opts.UseNpgsql(builder.Configuration.GetConnectionString("PeopleDb"))
        .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>()));

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
}

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors(myAllowSpecificOrigins);

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<PeopleDbContext>();
        if (!db.Database.ProviderName!.Contains("InMemory"))
            db.Database.Migrate();
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }