using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using SchedlifyApi.Models;
using SchedlifyApi.Repositories;
using SchedlifyApi.Controllers;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Microsoft.AspNetCore.Http;
using System.Runtime.Intrinsics.Arm;


namespace SchedlifyApi.Services;

public class TgDailyMessage: BackgroundService
{
    private int dailyPushHour = 18;
    private int dailyPushMinute = 0;
    ITelegramBotClient _botClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private IAssignmentRepository _assignmentRepository;
    private ITemplateSlotRepository _templateSlotRepository;
    private IClassRepository _classRepository;
    private ITgUserRepository _tgUserRepository;
    private IGroupRepository _groupRepository;
    private IDepartmentRepository _departmentRepository;
    private IUniversityRepository _universityRepository;
    private AssignmentController _assignmentController;
    private ClassesController _classesController;
    private string scheduleDay_message;
    
    public TgDailyMessage(
        ITelegramBotClient botClient,
        IServiceScopeFactory scopeFactory
        )
    {
        scheduleDay_message =
"📅 Дата: {0}\n" +
"🗓️ День тижня: {1}\n\n" +
"🔍 Натисни на предмет, щоб побачити детальну інформацію.\n\n" +
"🎓 Університет: {2}\n" +
"🏛️ Факультет: {3}\n" +
"👥 Група: {4}";
        _botClient = botClient;
        _scopeFactory = scopeFactory;
        
        var scope = _scopeFactory.CreateScope();
        _assignmentRepository = scope.ServiceProvider.GetRequiredService<IAssignmentRepository>();
        _templateSlotRepository = scope.ServiceProvider.GetRequiredService<ITemplateSlotRepository>();
        _classRepository = scope.ServiceProvider.GetRequiredService<IClassRepository>();
        _tgUserRepository = scope.ServiceProvider.GetRequiredService<ITgUserRepository>();
        _groupRepository = scope.ServiceProvider.GetRequiredService<IGroupRepository>();
        _departmentRepository = scope.ServiceProvider.GetRequiredService<IDepartmentRepository>();
        _universityRepository = scope.ServiceProvider.GetRequiredService<IUniversityRepository>();
        _assignmentController = new AssignmentController(_assignmentRepository, _templateSlotRepository, _classRepository);
        _classesController = new ClassesController(_classRepository, _groupRepository);
    }


    private async Task sendDailyMessage(List<Assignment> assignments, TgUser tgUser, DateOnly date)
    {
        int a = date.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber;
        string dateString = $"{date.Day} {Dicts.Months[date.Month]}";
        Weekday kyivWeekday = (Weekday)(((int)date.DayOfWeek + 6) % 7);
        string dayWeek = Dicts.WeekDays[kyivWeekday];
        Group? group = await _groupRepository.GetById(tgUser.GroupId);
        Department? department = await _departmentRepository.GetById(group.DepartmentId);
        University? university = await _universityRepository.GetById(department.UniversityId);
        List<List<InlineKeyboardButton>> buttonList = new List<List<InlineKeyboardButton>>();
        foreach (Assignment assignment in assignments)
        {
            buttonList.Add(
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton(
                        $"{assignment.StartTime}: {(await _classesController.GetClassByIdInner(assignment.ClassId)).Name}",
                        $"showAssignmentInfo,{a},{assignment.Id}"
                    )
                }
            );

        }

        int b = 1;
        int c = 1;
        if (date.DayOfWeek == DayOfWeek.Monday)
        {
            b = 2;
        }

        if (date.DayOfWeek == DayOfWeek.Saturday)
        {
            c = 2;
        }

        buttonList.Add(new List<InlineKeyboardButton>
        {

            new InlineKeyboardButton("<-", $"show,{a - b}"),
            new InlineKeyboardButton("->", $"show,{a + c}")
        });

        buttonList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("🔽Приховати", "hideMessage") });
        try
        {
            await _botClient.SendMessage(tgUser.Id, string.Format(scheduleDay_message, dateString, dayWeek, university.Name, department.Name, group.Name),
                replyMarkup: new InlineKeyboardMarkup(buttonList));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    private DateTime GetLocalNow()
    {
        TimeZoneInfo kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        DateTime kyivDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kyivTimeZone);
        return kyivDateTime;
    }

    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime kyivDateTime = GetLocalNow() + TimeSpan.FromHours(24);
            TimeOnly kyivTimeOnly = TimeOnly.FromDateTime(kyivDateTime);
            DateOnly kyivDate = DateOnly.FromDateTime(kyivDateTime);
            Weekday kyivWeekday = (Weekday)(((int)kyivDateTime.DayOfWeek + 6) % 7);
            
            Console.WriteLine(kyivTimeOnly.Hour);
            Console.WriteLine(kyivTimeOnly.Minute);
            if (kyivTimeOnly.Hour == dailyPushHour && kyivTimeOnly.Minute == dailyPushMinute)
            {
                List<TgUser> tgUsers = await _tgUserRepository.GetAllAsync();
                foreach (TgUser tgUser in tgUsers)
                {
                    if (tgUser.GroupId is not null && tgUser.Subscribed)
                    {
                        List<Assignment> assignments = await _assignmentController.GetByGroupIdAndDateInner((int)tgUser.GroupId, kyivDate);
                        if (assignments.Count == 0) { continue; }
                        await sendDailyMessage(assignments, tgUser, kyivDate);
                    }
                }
            }

        await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}