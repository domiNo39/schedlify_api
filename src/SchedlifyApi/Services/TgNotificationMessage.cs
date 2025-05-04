using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using SchedlifyApi.Models;
using SchedlifyApi.Repositories;
using Telegram.Bot;

namespace SchedlifyApi.Services;


public class TgNotificationMessage: BackgroundService
{
    private int minutesBeforeClassStarts = 15;
    ITelegramBotClient _botClient;
    private readonly IServiceScopeFactory _scopeFactory;
    public TgNotificationMessage(
        ITelegramBotClient botClient,
        IServiceScopeFactory scopeFactory
        )
    {
        _botClient = botClient;
        _scopeFactory = scopeFactory;
    }
    
    private static bool IsEvenWeek(DateOnly date)
    {
        int weekOfYear = (date.DayOfYear - 1) / 7 + 1;
        return weekOfYear % 2 == 0;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var _assignmentRepository = scope.ServiceProvider.GetRequiredService<IAssignmentRepository>();
        var tgUserRepo = scope.ServiceProvider.GetRequiredService<ITgUserRepository>();
        while (!stoppingToken.IsCancellationRequested)
        {
            TimeZoneInfo kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            DateTime kyivTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kyivTimeZone);
            TimeOnly kyivTimeOnly = TimeOnly.FromDateTime(kyivTime);
            DateOnly kyivDate = DateOnly.FromDateTime(kyivTime);
            Weekday kyivWeekday = (Weekday)(((int)kyivTime.DayOfWeek + 6) % 7);
            List<Assignment> specialAssignments = await _assignmentRepository.GetAllAssignmentsByDate(
                kyivDate
                );
            
            List<Assignment> regularAssignments = await _assignmentRepository.GetAllAssignmentsByWeekday(kyivWeekday, AssignmentType.Regular);

            AssignmentType currentAssignmentType = IsEvenWeek(kyivDate) ? AssignmentType.Even : AssignmentType.Odd;
            
            List<Assignment> evenOddAssignments = await _assignmentRepository.GetAllAssignmentsByWeekday(kyivWeekday, currentAssignmentType);
            
            List<Assignment> assignments = regularAssignments.Concat(evenOddAssignments).ToList().Concat(specialAssignments).ToList();

            foreach (Assignment assignment in assignments)
            {
                if ((int)(assignment.StartTime - kyivTimeOnly).TotalMinutes == minutesBeforeClassStarts)
                {
                    List<TgUser> tgUsers = await tgUserRepo.GetByGroupIdAsync(assignment.GroupId);
                    foreach (TgUser tgUser in tgUsers)
                    {
                        string messageText = $"Заняття {assignment.Class.Name} розпочинається через {minutesBeforeClassStarts} хв";
                        if (tgUser.Subscribed)
                        {
                            try
                            {
                                await _botClient.SendMessage(
                                    tgUser.Id, messageText
                                );
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}