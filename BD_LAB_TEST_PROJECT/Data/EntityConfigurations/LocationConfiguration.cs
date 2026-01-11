using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BD_LAB_TEST_PROJECT.Data.EntityConfigurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("locations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.HasIndex(x => new { x.TravelPlanId, x.Order })
                .IsUnique();

            builder.Property(x => x.Version)
            .IsConcurrencyToken();

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("now() at time zone 'utc'");

            builder.Property(x => x.UpdatedAt)
                .HasDefaultValueSql("now() at time zone 'utc'");
        }
    }
}
