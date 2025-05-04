using Microsoft.EntityFrameworkCore;
using SchedlifyApi.Data;
using SchedlifyApi.Repositories;
using SchedlifyApi.Middleware;
using SchedlifyApi.Services;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DB context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    string token = config["TelegramBot:Token"];
    if (string.IsNullOrEmpty(token))
    {
        throw new ApplicationException("TelegramBot token is missing");
    }
    return new TelegramBotClient(token);
});

// Register the repository
builder.Services.AddScoped<ITgUserRepository, TgUserRepository>();
builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<ITemplateSlotRepository, TemplateSlotRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddHostedService<TelegramMessagesService>();

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