using System.ComponentModel;

namespace Common.Enums;

public enum TicketStatusEnum
{
    [Description("جدید")] New = 1,
    [Description("پاسخ استاد")] TeacherResponse = 2,
    [Description("پاسخ دانشجو")] UserResponse = 3,
    [Description("بسته")] Closed = 4,
}