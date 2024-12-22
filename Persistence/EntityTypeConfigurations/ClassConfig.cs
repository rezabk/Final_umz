using Domain.Entities.ClassEntities;
using Domain.Entities.CommunityEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeConfigurations;

public class ClassConfig : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.HasOne(c => c.Community)
            .WithOne(c => c.Class)
            .HasForeignKey<Community>(c => c.ClassId);
    }
}