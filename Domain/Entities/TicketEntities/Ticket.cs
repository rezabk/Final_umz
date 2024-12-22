using Common.Enums;
using Domain.Attributes;
using Domain.Entities.BaseAgg;
using Domain.Entities.ClassEntities;
using Domain.Entities.TeacherEntities;
using Domain.Entities.UserEntities;

namespace Domain.Entities.TicketEntities;

[EntityType]
[Auditable]
public class Ticket : EntityBaseKey<int>
{
    public int UserId { get; set; }

    public virtual ApplicationUser User { get; set; }

    public int TeacherId { get; set; }

    public virtual Teacher Teacher { get; set; }

    public int ClassId { get; set; }

    public virtual Class Class { get; set; }

    public string Subject { get; set; }

    public DateTime SentTime { get; set; }
    
    public DateTime? CloseTime { get; set; }

    public int OpenByUserId { get; set; }

    public int ClosedByUserId { get; set; }
    
    public string? FileName { get; set; }
    
    public string? FileExtension { get; set; }
    
    public TicketStatusEnum Status { get; set; }
    
    public virtual ICollection<TicketMessage> TicketMessages { get; set; }
}