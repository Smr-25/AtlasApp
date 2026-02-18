using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class OnboardingOptionConfiguration : IEntityTypeConfiguration<OnboardingOption>
{
    private static readonly DateTimeOffset SeedDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public void Configure(EntityTypeBuilder<OnboardingOption> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Text).IsRequired().HasMaxLength(200);
        builder.Property(o => o.QuestionId).IsRequired();

        builder.HasData(
            // ==================== PROFESSION OPTIONS (Q_Profession) ====================
            new
            {
                Id = new Guid("aaaa0001-0001-0001-0001-000000000001"),
                Text = "Developer",
                QuestionId = OnboardingQuestionConfiguration.Q_Profession,
                RecommendedIntegration = "GitHub",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("aaaa0001-0001-0001-0001-000000000002"),
                Text = "Designer",
                QuestionId = OnboardingQuestionConfiguration.Q_Profession,
                RecommendedIntegration = "Figma",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("aaaa0001-0001-0001-0001-000000000003"),
                Text = "DevOps Engineer",
                QuestionId = OnboardingQuestionConfiguration.Q_Profession,
                RecommendedIntegration = "Docker",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("aaaa0001-0001-0001-0001-000000000004"),
                Text = "Data Scientist",
                QuestionId = OnboardingQuestionConfiguration.Q_Profession,
                RecommendedIntegration = "Jupyter",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("aaaa0001-0001-0001-0001-000000000005"),
                Text = "Cyber Security Specialist",
                QuestionId = OnboardingQuestionConfiguration.Q_Profession,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("aaaa0001-0001-0001-0001-000000000006"),
                Text = "AI/ML Engineer",
                QuestionId = OnboardingQuestionConfiguration.Q_Profession,
                RecommendedIntegration = "OpenAI",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("aaaa0001-0001-0001-0001-000000000007"),
                Text = "Product Manager",
                QuestionId = OnboardingQuestionConfiguration.Q_Profession,
                RecommendedIntegration = "Jira",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("aaaa0001-0001-0001-0001-000000000008"),
                Text = "Other",
                QuestionId = OnboardingQuestionConfiguration.Q_Profession,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== MAIN GOALS OPTIONS (Q_MainGoals - shown to everyone) ====================
            new
            {
                Id = new Guid("aaaa0002-0002-0002-0002-000000000001"),
                Text = "Improve productivity",
                QuestionId = OnboardingQuestionConfiguration.Q_MainGoals,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("aaaa0002-0002-0002-0002-000000000002"),
                Text = "Organize my work better",
                QuestionId = OnboardingQuestionConfiguration.Q_MainGoals,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("aaaa0002-0002-0002-0002-000000000003"),
                Text = "Collaborate with team",
                QuestionId = OnboardingQuestionConfiguration.Q_MainGoals,
                RecommendedIntegration = "Slack",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("aaaa0002-0002-0002-0002-000000000004"),
                Text = "Automate repetitive tasks",
                QuestionId = OnboardingQuestionConfiguration.Q_MainGoals,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("aaaa0002-0002-0002-0002-000000000005"),
                Text = "Track projects and deadlines",
                QuestionId = OnboardingQuestionConfiguration.Q_MainGoals,
                RecommendedIntegration = "Jira",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== DEVELOPER - LANGUAGES (Q_Dev_Languages) ====================
            new
            {
                Id = new Guid("bbbb0001-0001-0001-0001-000000000001"),
                Text = "JavaScript / TypeScript",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Languages,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0001-0001-0001-0001-000000000002"),
                Text = "Python",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Languages,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0001-0001-0001-0001-000000000003"),
                Text = "C# / .NET",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Languages,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0001-0001-0001-0001-000000000004"),
                Text = "Java / Kotlin",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Languages,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0001-0001-0001-0001-000000000005"),
                Text = "Go",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Languages,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0001-0001-0001-0001-000000000006"),
                Text = "Rust",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Languages,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0001-0001-0001-0001-000000000007"),
                Text = "PHP",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Languages,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0001-0001-0001-0001-000000000008"),
                Text = "Swift / Objective-C",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Languages,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0001-0001-0001-0001-000000000009"),
                Text = "Ruby",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Languages,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0001-0001-0001-0001-000000000010"),
                Text = "C / C++",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Languages,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== DEVELOPER - TOOLS (Q_Dev_Tools) ====================
            new
            {
                Id = new Guid("bbbb0002-0002-0002-0002-000000000001"),
                Text = "VS Code",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0002-0002-0002-0002-000000000002"),
                Text = "JetBrains IDEs",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0002-0002-0002-0002-000000000003"),
                Text = "GitHub",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Tools,
                RecommendedIntegration = "GitHub",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0002-0002-0002-0002-000000000004"),
                Text = "GitLab",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Tools,
                RecommendedIntegration = "GitLab",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0002-0002-0002-0002-000000000005"),
                Text = "Docker",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Tools,
                RecommendedIntegration = "Docker",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0002-0002-0002-0002-000000000006"),
                Text = "Postman",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== DEVELOPER - FRAMEWORKS (Q_Dev_Frameworks) ====================
            new
            {
                Id = new Guid("bbbb0003-0003-0003-0003-000000000001"),
                Text = "React / Next.js",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0003-0003-0003-0003-000000000002"),
                Text = "Angular",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0003-0003-0003-0003-000000000003"),
                Text = "Vue.js / Nuxt",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0003-0003-0003-0003-000000000004"),
                Text = "ASP.NET Core",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0003-0003-0003-0003-000000000005"),
                Text = "Node.js / Express",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0003-0003-0003-0003-000000000006"),
                Text = "Django / Flask",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0003-0003-0003-0003-000000000007"),
                Text = "Spring Boot",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0003-0003-0003-0003-000000000008"),
                Text = "Laravel",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("bbbb0003-0003-0003-0003-000000000009"),
                Text = "React Native / Flutter",
                QuestionId = OnboardingQuestionConfiguration.Q_Dev_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== DESIGNER - TOOLS (Q_Design_Tools) ====================
            new
            {
                Id = new Guid("cccc0001-0001-0001-0001-000000000001"),
                Text = "Figma",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Tools,
                RecommendedIntegration = "Figma",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0001-0001-0001-0001-000000000002"),
                Text = "Adobe XD",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0001-0001-0001-0001-000000000003"),
                Text = "Sketch",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0001-0001-0001-0001-000000000004"),
                Text = "Adobe Photoshop",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0001-0001-0001-0001-000000000005"),
                Text = "Adobe Illustrator",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0001-0001-0001-0001-000000000006"),
                Text = "Canva",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0001-0001-0001-0001-000000000007"),
                Text = "Framer",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0001-0001-0001-0001-000000000008"),
                Text = "Blender (3D)",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== DESIGNER - SPECIALIZATION (Q_Design_Specialization) ====================
            new
            {
                Id = new Guid("cccc0002-0002-0002-0002-000000000001"),
                Text = "UI/UX Design",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Specialization,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0002-0002-0002-0002-000000000002"),
                Text = "Graphic Design",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Specialization,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0002-0002-0002-0002-000000000003"),
                Text = "Motion Design",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Specialization,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0002-0002-0002-0002-000000000004"),
                Text = "Brand Design",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Specialization,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0002-0002-0002-0002-000000000005"),
                Text = "3D Design",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Specialization,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("cccc0002-0002-0002-0002-000000000006"),
                Text = "Web Design",
                QuestionId = OnboardingQuestionConfiguration.Q_Design_Specialization,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== DEVOPS - CLOUD (Q_DevOps_Cloud) ====================
            new
            {
                Id = new Guid("dddd0001-0001-0001-0001-000000000001"),
                Text = "AWS",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_Cloud,
                RecommendedIntegration = "AWS",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("dddd0001-0001-0001-0001-000000000002"),
                Text = "Azure",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_Cloud,
                RecommendedIntegration = "Azure",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("dddd0001-0001-0001-0001-000000000003"),
                Text = "Google Cloud",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_Cloud,
                RecommendedIntegration = "GoogleCloud",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("dddd0001-0001-0001-0001-000000000004"),
                Text = "DigitalOcean",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_Cloud,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("dddd0001-0001-0001-0001-000000000005"),
                Text = "Kubernetes",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_Cloud,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== DEVOPS - CI/CD (Q_DevOps_CICD) ====================
            new
            {
                Id = new Guid("dddd0002-0002-0002-0002-000000000001"),
                Text = "GitHub Actions",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_CICD,
                RecommendedIntegration = "GitHub",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("dddd0002-0002-0002-0002-000000000002"),
                Text = "GitLab CI/CD",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_CICD,
                RecommendedIntegration = "GitLab",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("dddd0002-0002-0002-0002-000000000003"),
                Text = "Jenkins",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_CICD,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("dddd0002-0002-0002-0002-000000000004"),
                Text = "Azure DevOps",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_CICD,
                RecommendedIntegration = "Azure",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("dddd0002-0002-0002-0002-000000000005"),
                Text = "CircleCI",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_CICD,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("dddd0002-0002-0002-0002-000000000006"),
                Text = "Docker",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_CICD,
                RecommendedIntegration = "Docker",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("dddd0002-0002-0002-0002-000000000007"),
                Text = "Terraform",
                QuestionId = OnboardingQuestionConfiguration.Q_DevOps_CICD,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== DATA SCIENTIST - TOOLS (Q_Data_Tools) ====================
            new
            {
                Id = new Guid("eeee0001-0001-0001-0001-000000000001"),
                Text = "Python / Pandas",
                QuestionId = OnboardingQuestionConfiguration.Q_Data_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("eeee0001-0001-0001-0001-000000000002"),
                Text = "Jupyter Notebooks",
                QuestionId = OnboardingQuestionConfiguration.Q_Data_Tools,
                RecommendedIntegration = "Jupyter",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("eeee0001-0001-0001-0001-000000000003"),
                Text = "TensorFlow / PyTorch",
                QuestionId = OnboardingQuestionConfiguration.Q_Data_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("eeee0001-0001-0001-0001-000000000004"),
                Text = "SQL / Databases",
                QuestionId = OnboardingQuestionConfiguration.Q_Data_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("eeee0001-0001-0001-0001-000000000005"),
                Text = "Tableau / Power BI",
                QuestionId = OnboardingQuestionConfiguration.Q_Data_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("eeee0001-0001-0001-0001-000000000006"),
                Text = "Apache Spark",
                QuestionId = OnboardingQuestionConfiguration.Q_Data_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("eeee0001-0001-0001-0001-000000000007"),
                Text = "R / RStudio",
                QuestionId = OnboardingQuestionConfiguration.Q_Data_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== CYBERSECURITY - FOCUS (Q_Security_Focus) ====================
            new
            {
                Id = new Guid("ffff0001-0001-0001-0001-000000000001"),
                Text = "Penetration Testing",
                QuestionId = OnboardingQuestionConfiguration.Q_Security_Focus,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("ffff0001-0001-0001-0001-000000000002"),
                Text = "Network Security",
                QuestionId = OnboardingQuestionConfiguration.Q_Security_Focus,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("ffff0001-0001-0001-0001-000000000003"),
                Text = "Application Security",
                QuestionId = OnboardingQuestionConfiguration.Q_Security_Focus,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("ffff0001-0001-0001-0001-000000000004"),
                Text = "Cloud Security",
                QuestionId = OnboardingQuestionConfiguration.Q_Security_Focus,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("ffff0001-0001-0001-0001-000000000005"),
                Text = "Incident Response",
                QuestionId = OnboardingQuestionConfiguration.Q_Security_Focus,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("ffff0001-0001-0001-0001-000000000006"),
                Text = "Security Operations (SOC)",
                QuestionId = OnboardingQuestionConfiguration.Q_Security_Focus,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== AI ENGINEER - FRAMEWORKS (Q_AI_Frameworks) ====================
            new
            {
                Id = new Guid("1111a001-0001-0001-0001-000000000001"),
                Text = "OpenAI / GPT",
                QuestionId = OnboardingQuestionConfiguration.Q_AI_Frameworks,
                RecommendedIntegration = "OpenAI",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("1111a001-0001-0001-0001-000000000002"),
                Text = "TensorFlow",
                QuestionId = OnboardingQuestionConfiguration.Q_AI_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("1111a001-0001-0001-0001-000000000003"),
                Text = "PyTorch",
                QuestionId = OnboardingQuestionConfiguration.Q_AI_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("1111a001-0001-0001-0001-000000000004"),
                Text = "LangChain",
                QuestionId = OnboardingQuestionConfiguration.Q_AI_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("1111a001-0001-0001-0001-000000000005"),
                Text = "Hugging Face",
                QuestionId = OnboardingQuestionConfiguration.Q_AI_Frameworks,
                RecommendedIntegration = "HuggingFace",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("1111a001-0001-0001-0001-000000000006"),
                Text = "Anthropic / Claude",
                QuestionId = OnboardingQuestionConfiguration.Q_AI_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("1111a001-0001-0001-0001-000000000007"),
                Text = "Stable Diffusion / DALL-E",
                QuestionId = OnboardingQuestionConfiguration.Q_AI_Frameworks,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            
            // ==================== PRODUCT MANAGER - TOOLS (Q_PM_Tools) ====================
            new
            {
                Id = new Guid("2222a001-0001-0001-0001-000000000001"),
                Text = "Jira",
                QuestionId = OnboardingQuestionConfiguration.Q_PM_Tools,
                RecommendedIntegration = "Jira",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("2222a001-0001-0001-0001-000000000002"),
                Text = "Asana",
                QuestionId = OnboardingQuestionConfiguration.Q_PM_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("2222a001-0001-0001-0001-000000000003"),
                Text = "Trello",
                QuestionId = OnboardingQuestionConfiguration.Q_PM_Tools,
                RecommendedIntegration = "Trello",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("2222a001-0001-0001-0001-000000000004"),
                Text = "Linear",
                QuestionId = OnboardingQuestionConfiguration.Q_PM_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("2222a001-0001-0001-0001-000000000005"),
                Text = "Notion",
                QuestionId = OnboardingQuestionConfiguration.Q_PM_Tools,
                RecommendedIntegration = "Notion",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("2222a001-0001-0001-0001-000000000006"),
                Text = "Confluence",
                QuestionId = OnboardingQuestionConfiguration.Q_PM_Tools,
                RecommendedIntegration = "Confluence",
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("2222a001-0001-0001-0001-000000000007"),
                Text = "Monday.com",
                QuestionId = OnboardingQuestionConfiguration.Q_PM_Tools,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new
            {
                Id = new Guid("2222a001-0001-0001-0001-000000000008"),
                Text = "Slack",
                QuestionId = OnboardingQuestionConfiguration.Q_PM_Tools,
                RecommendedIntegration = "Slack",
                CreatedAt = SeedDate,
                IsDeleted = false

            }
        );
    }
}