using SchedlifyApi.Models;

public class Dicts
{
    public static readonly Dictionary<Mode, string> Modes = new Dictionary<Mode, string>()
    {
        {Mode.Online, "�����������"},
        {Mode.Offline, "����" }
    };
    public static readonly Dictionary<Weekday, string> WeekDays = new Dictionary<Weekday, string>()
    {
        {Weekday.Monday, "��������" },
        {Weekday.Tuesday, "³������" },
        {Weekday.Wednesday,"������" },
        {Weekday.Thursday, "������" },
        {Weekday.Friday, "�'������" },
        {Weekday.Saturday, "������" },
        {Weekday.Sunday, "�����" }
    };
    public static readonly Dictionary<ClassType, string> ClassTypes = new Dictionary<ClassType, string>()
    {
        {ClassType.Lecture, "������" },
        {ClassType.Seminar, "���������" }
    };
    public static readonly Dictionary<AssignmentType, string> AssignmentTypes = new Dictionary<AssignmentType, string>()
    {
        {AssignmentType.Special, "����������"},
        {AssignmentType.Regular, "���������"},
        {AssignmentType.Odd, "���������"},
        {AssignmentType.Even, "���������"}
    };
    public static readonly Dictionary<int, string> Months = new Dictionary<int, string>()
    {
        {1, "����"},
        {2, "������"},
        {3, "�������"},
        {4, "�����"},
        {5, "������"},
        {6, "������"},
        {7, "�����"},
        {8, "������"},
        {9, "�������"},
        {10, "������"},
        {11, "���������"},
        {12, "������"}
    };
}