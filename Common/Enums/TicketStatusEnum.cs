using System.ComponentModel;

namespace Common.Enums;

public enum TicketStatusEnum
{
    [Description("جدید توسط دانشجو")] NewByUser = 1,
    [Description("جدید توسط استاد")] NewByTeacher = 2,
    [Description("پاسخ استاد")] TeacherResponse = 3,
    [Description("پاسخ دانشجو")] UserResponse = 4,
    [Description("بسته")] Closed = 5,
}