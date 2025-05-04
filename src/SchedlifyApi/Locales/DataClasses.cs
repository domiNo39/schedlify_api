using SchedlifyApi.Models;

public class Dicts
{
    public static readonly Dictionary<Mode, string> Modes = new Dictionary<Mode, string>()
    {
        {Mode.Online, "Дистанційно"},
        {Mode.Offline, "Очно" }
    };
    public static readonly Dictionary<Weekday, string> WeekDays = new Dictionary<Weekday, string>()
    {
        {Weekday.Monday, "Понеділок" },
        {Weekday.Tuesday, "Вівторок" },
        {Weekday.Wednesday,"Середа" },
        {Weekday.Thursday, "Четвер" },
        {Weekday.Friday, "П'ятниця" },
        {Weekday.Saturday, "Субота" },
        {Weekday.Sunday, "Неділя" }
    };
    public static readonly Dictionary<ClassType, string> ClassTypes = new Dictionary<ClassType, string>()
    {
        {ClassType.Lecture, "Лекція" },
        {ClassType.Seminar, "Практична" }
    };
    public static readonly Dictionary<AssignmentType, string> AssignmentTypes = new Dictionary<AssignmentType, string>()
    {
        {AssignmentType.Special, "Одноразова"},
        {AssignmentType.Regular, "Регулярна"},
        {AssignmentType.Odd, "Періодична"},
        {AssignmentType.Even, "Періодична"}
    };
    public static readonly Dictionary<int, string> Months = new Dictionary<int, string>()
    {
        {1, "січня"},
        {2, "лютого"},
        {3, "березня"},
        {4, "квітня"},
        {5, "травня"},
        {6, "червня"},
        {7, "липня"},
        {8, "серпня"},
        {9, "вересня"},
        {10, "жовтня"},
        {11, "листопада"},
        {12, "грудня"}
    };
}