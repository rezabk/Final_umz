using Domain.Attributes;
using Domain.Entities.BaseAgg;
using Domain.Entities.ClassEntities;
using Domain.Entities.TeacherEntities;

namespace Domain.Entities.CommunityEntities;


[EntityType]
[Auditable]
public class Community : EntityBaseKeyInteger
{
    public int ClassId { get; set; }
    
    public virtual Class Class { get; set; }
    
    public int TeacherId { get; set; }
    
    public virtual Teacher Teacher { get; set; }
    
    public List<CommunityMessage> CommunityMessages { get; set; }

}