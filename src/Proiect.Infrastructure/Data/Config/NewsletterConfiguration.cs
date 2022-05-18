using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proiect.Core.Entities;

namespace Proiect.Infrastructure.Data.Config
{
    public class NewsletterConfiguration : IEntityTypeConfiguration<NewsletterSubscription>
    {
        public void Configure(EntityTypeBuilder<NewsletterSubscription> builder)
        {
            builder.HasIndex(t => t.EmailAddress)
                .IsUnique();
        }
    }
}
