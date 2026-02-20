using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class OnboardingQuestionConfiguration : IEntityTypeConfiguration<OnboardingQuestion>
{
    
    public static readonly Guid Q_Profession = new("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Q_MainGoals = new("11111111-1111-1111-1111-111111111112");
    
    public static readonly Guid Q_Dev_Languages = new("22222222-2222-2222-2222-222222222201");
    public static readonly Guid Q_Dev_Tools = new("22222222-2222-2222-2222-222222222202");
    public static readonly Guid Q_Dev_Frameworks = new("22222222-2222-2222-2222-222222222203");
    
    public static readonly Guid Q_Design_Tools = new("33333333-3333-3333-3333-333333333301");
    public static readonly Guid Q_Design_Specialization = new("33333333-3333-3333-3333-333333333302");
    
    public static readonly Guid Q_DevOps_Cloud = new("44444444-4444-4444-4444-444444444401");
    public static readonly Guid Q_DevOps_CICD = new("44444444-4444-4444-4444-444444444402");
    
    public static readonly Guid Q_Data_Tools = new("55555555-5555-5555-5555-555555555501");
    
    public static readonly Guid Q_Security_Focus = new("66666666-6666-6666-6666-666666666601");
    
    public static readonly Guid Q_AI_Frameworks = new("77777777-7777-7777-7777-777777777701");
    
    public static readonly Guid Q_PM_Tools = new("88888888-8888-8888-8888-888888888801");
    
    private static readonly DateTimeOffset SeedDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public void Configure(EntityTypeBuilder<OnboardingQuestion> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Text).IsRequired().HasMaxLength(500);
        builder.Property(q => q.TargetProfession);
        builder.HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasData(
            new 
            { 
                Id = Q_Profession, 
                Text = "What is your profession?", 
                Order = 1, 
                IsMultiSelect = false,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new 
            { 
                Id = Q_MainGoals, 
                Text = "What are your main goals for using Atlas?", 
                Order = 2, 
                IsMultiSelect = true,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            new 
            { 
                Id = Q_Dev_Languages, 
                Text = "Which programming languages do you primarily work with?", 
                Order = 3, 
                IsMultiSelect = true,
                TargetProfession = UserProfession.Developer,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new 
            { 
                Id = Q_Dev_Tools, 
                Text = "Which development tools do you use?", 
                Order = 4, 
                IsMultiSelect = true,
                TargetProfession = UserProfession.Developer,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new 
            { 
                Id = Q_Dev_Frameworks, 
                Text = "Which frameworks/libraries are you most experienced with?", 
                Order = 5, 
                IsMultiSelect = true,
                TargetProfession = UserProfession.Developer,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            new 
            { 
                Id = Q_Design_Tools, 
                Text = "Which design tools do you use?", 
                Order = 3, 
                IsMultiSelect = true,
                TargetProfession = UserProfession.Designer,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new 
            { 
                Id = Q_Design_Specialization, 
                Text = "What is your design specialization?", 
                Order = 4, 
                IsMultiSelect = true,
                TargetProfession = UserProfession.Designer,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            new 
            { 
                Id = Q_DevOps_Cloud, 
                Text = "Which cloud platforms do you work with?", 
                Order = 3, 
                IsMultiSelect = true,
                TargetProfession = UserProfession.Developer,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new 
            { 
                Id = Q_DevOps_CICD, 
                Text = "Which CI/CD tools do you use?", 
                Order = 4, 
                IsMultiSelect = true,
                TargetProfession = UserProfession.CyberSecurity,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            new 
            { 
                Id = Q_Data_Tools, 
                Text = "Which data science tools and libraries do you use?", 
                Order = 3, 
                IsMultiSelect = true,
                TargetProfession = UserProfession.Developer,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            new 
            { 
                Id = Q_Security_Focus, 
                Text = "What is your security focus area?", 
                Order = 3, 
                IsMultiSelect = true,
                TargetProfession = UserProfession.CyberSecurity,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            new 
            { 
                Id = Q_AI_Frameworks, 
                Text = "Which AI/ML frameworks do you work with?", 
                Order = 3, 
                IsMultiSelect = true,
                TargetProfession = UserProfession.DigitalMarketing,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            new 
            { 
                Id = Q_PM_Tools, 
                Text = "Which project management tools do you use?", 
                Order = 3, 
                IsMultiSelect = true,
                TargetProfession = UserProfession.ProductManager,
                CreatedAt = SeedDate,
                IsDeleted = false
            }
        );

    }
}