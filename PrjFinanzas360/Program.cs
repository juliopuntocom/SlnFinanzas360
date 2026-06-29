using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PrjFinanzas360.Data;
using PrjFinanzas360.Engine;
using PrjFinanzas360.Interface;
using PrjFinanzas360.Security;
using PrjFinanzas360.Services;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

//Services
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<GastoService>();
builder.Services.AddScoped<MenuService>();

builder.Services.AddScoped<IOcrEngine, TesseractOcrEngine>();
builder.Services.AddScoped<OcrProcesor>();
builder.Services.AddScoped<OcrService>();
builder.Services.AddScoped<PdfToImageConverter>();

builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<MetodoPagoService>();

builder.Services.AddScoped<IDashboardService, DashboardService>();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// JWT Authentication
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

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        )
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Swagger / OpenAPI
builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
