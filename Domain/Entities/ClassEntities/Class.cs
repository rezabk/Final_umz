﻿using System.ComponentModel.DataAnnotations.Schema;
using Common.Enums.ClassEnum;
using Domain.Attributes;
using Domain.Entities.BaseAgg;
using Domain.Entities.CommunityEntities;
using Domain.Entities.PracticeEntities;
using Domain.Entities.TeacherEntities;
using Domain.Entities.UserAgg;
using Domain.Entities.UserEntities;

namespace Domain.Entities.ClassEntities;

[EntityType]
[Auditable]
public class Class : EntityBaseKeyInteger
{
    public int TeacherId { get; set; }

    public virtual Teacher Teacher { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public string? UniversityName { get; set; }

    public int TotalAllowedStudent { get; set; }

    public ClassSemesterEnum Semester { get; set; }
    
    public int CommunityId { get; set; }
    
    public virtual Community Community { get; set; }


    public virtual ICollection<Practice> Practices { get; set; }

    public virtual ICollection<ApplicationUser> Students { get; set; }
    
    [Column("InsertTime")] public DateTime? InsertTime { get; set; }
}