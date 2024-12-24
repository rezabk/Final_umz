using System.ComponentModel;

namespace Common.Enums.RolesManagment;

public enum UserRolesEnum
{
    [Description("ادمین")] Admin = 1,
    [Description("استاد")] Teacher = 2,
    [Description("دانشجو")] Student = 3
}