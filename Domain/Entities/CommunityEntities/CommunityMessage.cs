using Common.Enums.RolesManagment;
using Domain.Attributes;
using Domain.Entities.BaseAgg;
using Domain.Entities.UserEntities;

namespace Domain.Entities.CommunityEntities;

[EntityType]
[Auditable]
public class CommunityMessage : EntityBaseKeyInteger
{
    public int CommunityId { get; set; }

    public virtual Community Community { get; set; }

    public string Message { get; set; }

    public int SentByUserId { get; set; }

    public virtual ApplicationUser SentByUser { get; set; }

    public UserRolesEnum SentByRole { get; set; }

    public DateTime SentTime { get; set; }
}