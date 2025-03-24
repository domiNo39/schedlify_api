using Microsoft.EntityFrameworkCore;
using SchedlifyApi.Data;
using SchedlifyApi.Repositories;
using SchedlifyApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DB context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the repository
builder.Services.AddScoped<ITgUserRepository, TgUserRepository>();
builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add the Telegram authentication middleware
// app.UseTelegramAuth();

app.UseAuthorization();

app.MapControllers();

app.Run();