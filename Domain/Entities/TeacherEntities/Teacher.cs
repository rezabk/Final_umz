using Common.Enums.TeacherEnum;
using Domain.Attributes;
using Domain.Entities.BaseAgg;
using Domain.Entities.ClassEntities;
using Domain.Entities.UserAgg;
using Domain.Entities.UserEntities;

namespace Domain.Entities.TeacherEntities;

[EntityType]
[Auditable]
public class Teacher : EntityBaseKeyInteger
{
    public int UserId { get; set; }

    public virtual ApplicationUser User { get; set; }

    public string? Description { get; set; }

    public string UniversityName { get; set; }
    
    public TeacherFieldEnum TeacherField { get; set; }

    public virtual ICollection<Class> Classes { get; set; }
}