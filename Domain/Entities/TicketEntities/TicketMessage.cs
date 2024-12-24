using Common.Enums.RolesManagment;
using Domain.Attributes;
using Domain.Entities.BaseAgg;
using Domain.Entities.UserEntities;

namespace Domain.Entities.TicketEntities;

[EntityType]
[Auditable]
public class TicketMessage :EntityBaseKey<int>
{
    public int TicketId { get; set; }
    
    public virtual Ticket Ticket { get; set; }
    
    public string? Message { get; set; }
    
    public string? FileName { get; set; }
    public string? FileExtension { get; set; }
    
    public int SentByUserId { get; set; }
    
    public virtual ApplicationUser SentByUser { get; set; }
    
    public UserRolesEnum SentByRole { get; set; }
    
    public DateTime SentTime { get; set; }
    
    
}