using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BD_LAB_TEST_PROJECT.Data.EntityConfigurations
{
    public class TravelPlanConfiguration : IEntityTypeConfiguration<TravelPlan>
    {
        public void Configure(EntityTypeBuilder<TravelPlan> builder)
        {
            builder.ToTable("travel_plans");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.BudgetEur)
                .HasDefaultValue(0);

            builder.Property(x => x.Version)
                .IsConcurrencyToken();

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("now() at time zone 'utc'");

            builder.Property(x => x.UpdatedAt)
                .HasDefaultValueSql("now() at time zone 'utc'");

            builder.HasMany(x => x.Locations)
                .WithOne(x => x.TravelPlan!)
                .HasForeignKey(x => x.TravelPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
