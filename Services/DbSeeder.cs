using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Services;

public static class DbSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "admin", "facadmin", "student" };

        foreach (var roleName in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string adminEmail = "djellal@univ-setif.dz";
        string adminPassword = "DhB@571982";
        string adminRole = "admin";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (createUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
        else
        {
            bool isInRole = await userManager.IsInRoleAsync(adminUser, adminRole);
            if (!isInRole)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }

    public static async Task SeedSampleUsersAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var sampleUsers = new[]
        {
            ("facadmin@univ-setif.dz", "FacAdmin@123", "facadmin"),
            ("student1@univ-setif.dz", "Student@123", "student"),
            ("student2@univ-setif.dz", "Student@123", "student"),
            ("student3@univ-setif.dz", "Student@123", "student"),
        };

        foreach (var (email, password, role) in sampleUsers)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }

    public static async Task SeedSampleFormsAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await context.Forms.AnyAsync())
            return;

        var adminUser = await userManager.FindByEmailAsync("djellal@univ-setif.dz");
        var adminId = adminUser?.Id;

        // Form 1: Student Registration
        var registrationForm = new Form
        {
            Title = "Student Registration Form",
            Description = "Register for the 2026 academic year. Please fill out all required fields.",
            Type = FormType.Registration,
            Status = FormStatus.Published,
            Slug = "student-registration-2026",
            CreatedByUserId = adminId,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };

        registrationForm.Fields = new List<FormField>
        {
            new() { Name = "first_name", Label = "First Name", FieldType = FieldType.Text, IsRequired = true, Order = 1, Placeholder = "Enter your first name" },
            new() { Name = "last_name", Label = "Last Name", FieldType = FieldType.Text, IsRequired = true, Order = 2, Placeholder = "Enter your last name" },
            new() { Name = "email", Label = "Email Address", FieldType = FieldType.Email, IsRequired = true, Order = 3, Placeholder = "student@example.com" },
            new() { Name = "phone", Label = "Phone Number", FieldType = FieldType.Phone, IsRequired = true, Order = 4, Placeholder = "+213 XXX XXX XXX" },
            new() { Name = "birth_date", Label = "Date of Birth", FieldType = FieldType.Date, IsRequired = true, Order = 5 },
            new() { Name = "gender", Label = "Gender", FieldType = FieldType.Radio, IsRequired = true, Order = 6, OptionsJson = "[{\"value\":\"male\",\"text\":\"Male\"},{\"value\":\"female\",\"text\":\"Female\"}]" },
            new() { Name = "faculty", Label = "Faculty", FieldType = FieldType.Select, IsRequired = true, Order = 7, OptionsJson = "[{\"value\":\"sciences\",\"text\":\"Faculty of Sciences\"},{\"value\":\"technology\",\"text\":\"Faculty of Technology\"},{\"value\":\"economics\",\"text\":\"Faculty of Economics\"},{\"value\":\"law\",\"text\":\"Faculty of Law\"},{\"value\":\"medicine\",\"text\":\"Faculty of Medicine\"}]" },
            new() { Name = "level", Label = "Study Level", FieldType = FieldType.Select, IsRequired = true, Order = 8, OptionsJson = "[{\"value\":\"l1\",\"text\":\"License 1\"},{\"value\":\"l2\",\"text\":\"License 2\"},{\"value\":\"l3\",\"text\":\"License 3\"},{\"value\":\"m1\",\"text\":\"Master 1\"},{\"value\":\"m2\",\"text\":\"Master 2\"},{\"value\":\"phd\",\"text\":\"Doctorate\"}]" },
            new() { Name = "address", Label = "Home Address", FieldType = FieldType.Textarea, IsRequired = false, Order = 9, Placeholder = "Enter your full address" },
            new() { Name = "photo", Label = "ID Photo", FieldType = FieldType.File, IsRequired = false, Order = 10, HelpText = "Upload a recent passport-sized photo (JPG, PNG)" },
        };

        context.Forms.Add(registrationForm);

        // Form 2: Course Satisfaction Survey
        var surveyForm = new Form
        {
            Title = "Course Satisfaction Survey",
            Description = "Help us improve our courses by sharing your feedback.",
            Type = FormType.Survey,
            Status = FormStatus.Published,
            Slug = "course-satisfaction-survey",
            CreatedByUserId = adminId,
            CreatedAt = DateTime.UtcNow.AddDays(-20),
            UpdatedAt = DateTime.UtcNow.AddDays(-2)
        };

        surveyForm.Fields = new List<FormField>
        {
            new() { Name = "course_name", Label = "Course Name", FieldType = FieldType.Text, IsRequired = true, Order = 1 },
            new() { Name = "instructor", Label = "Instructor Name", FieldType = FieldType.Text, IsRequired = true, Order = 2 },
            new() { Name = "overall_rating", Label = "Overall Course Rating", FieldType = FieldType.Radio, IsRequired = true, Order = 3, OptionsJson = "[{\"value\":\"5\",\"text\":\"Excellent\"},{\"value\":\"4\",\"text\":\"Very Good\"},{\"value\":\"3\",\"text\":\"Good\"},{\"value\":\"2\",\"text\":\"Fair\"},{\"value\":\"1\",\"text\":\"Poor\"}]" },
            new() { Name = "content_quality", Label = "Content Quality", FieldType = FieldType.Radio, IsRequired = true, Order = 4, OptionsJson = "[{\"value\":\"5\",\"text\":\"Excellent\"},{\"value\":\"4\",\"text\":\"Very Good\"},{\"value\":\"3\",\"text\":\"Good\"},{\"value\":\"2\",\"text\":\"Fair\"},{\"value\":\"1\",\"text\":\"Poor\"}]" },
            new() { Name = "teaching_quality", Label = "Teaching Quality", FieldType = FieldType.Radio, IsRequired = true, Order = 5, OptionsJson = "[{\"value\":\"5\",\"text\":\"Excellent\"},{\"value\":\"4\",\"text\":\"Very Good\"},{\"value\":\"3\",\"text\":\"Good\"},{\"value\":\"2\",\"text\":\"Fair\"},{\"value\":\"1\",\"text\":\"Poor\"}]" },
            new() { Name = "liked_aspects", Label = "What did you like about this course?", FieldType = FieldType.Checkbox, IsRequired = false, Order = 6, OptionsJson = "[{\"value\":\"content\",\"text\":\"Course Content\"},{\"value\":\"instructor\",\"text\":\"Instructor\"},{\"value\":\"materials\",\"text\":\"Learning Materials\"},{\"value\":\"practical\",\"text\":\"Practical Sessions\"},{\"value\":\"schedule\",\"text\":\"Schedule\"}]" },
            new() { Name = "comments", Label = "Additional Comments", FieldType = FieldType.Textarea, IsRequired = false, Order = 7, Placeholder = "Share any additional feedback..." },
            new() { Name = "recommend", Label = "Would you recommend this course?", FieldType = FieldType.Radio, IsRequired = true, Order = 8, OptionsJson = "[{\"value\":\"yes\",\"text\":\"Yes\"},{\"value\":\"no\",\"text\":\"No\"},{\"value\":\"maybe\",\"text\":\"Maybe\"}]" },
        };

        context.Forms.Add(surveyForm);

        // Form 3: Event Registration (Data Collection)
        var eventForm = new Form
        {
            Title = "Scientific Conference Registration",
            Description = "Register for the International Scientific Conference 2026.",
            Type = FormType.DataCollection,
            Status = FormStatus.Published,
            Slug = "conference-2026",
            CreatedByUserId = adminId,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        eventForm.Fields = new List<FormField>
        {
            new() { Name = "full_name", Label = "Full Name", FieldType = FieldType.Text, IsRequired = true, Order = 1 },
            new() { Name = "email", Label = "Email", FieldType = FieldType.Email, IsRequired = true, Order = 2 },
            new() { Name = "institution", Label = "Institution/University", FieldType = FieldType.Text, IsRequired = true, Order = 3 },
            new() { Name = "country", Label = "Country", FieldType = FieldType.Text, IsRequired = true, Order = 4 },
            new() { Name = "participation_type", Label = "Participation Type", FieldType = FieldType.Radio, IsRequired = true, Order = 5, OptionsJson = "[{\"value\":\"presenter\",\"text\":\"Paper Presenter\"},{\"value\":\"attendee\",\"text\":\"Attendee Only\"},{\"value\":\"poster\",\"text\":\"Poster Presenter\"}]" },
            new() { Name = "topics", Label = "Topics of Interest", FieldType = FieldType.Checkbox, IsRequired = true, Order = 6, OptionsJson = "[{\"value\":\"ai\",\"text\":\"Artificial Intelligence\"},{\"value\":\"ml\",\"text\":\"Machine Learning\"},{\"value\":\"data\",\"text\":\"Data Science\"},{\"value\":\"iot\",\"text\":\"Internet of Things\"},{\"value\":\"security\",\"text\":\"Cybersecurity\"}]" },
            new() { Name = "dietary", Label = "Dietary Requirements", FieldType = FieldType.Select, IsRequired = false, Order = 7, OptionsJson = "[{\"value\":\"none\",\"text\":\"None\"},{\"value\":\"vegetarian\",\"text\":\"Vegetarian\"},{\"value\":\"vegan\",\"text\":\"Vegan\"},{\"value\":\"halal\",\"text\":\"Halal\"},{\"value\":\"other\",\"text\":\"Other\"}]" },
            new() { Name = "abstract", Label = "Paper Abstract (if presenting)", FieldType = FieldType.Textarea, IsRequired = false, Order = 8, Placeholder = "Enter your paper abstract (max 500 words)" },
            new() { Name = "website", Label = "Personal/Academic Website", FieldType = FieldType.Url, IsRequired = false, Order = 9, Placeholder = "https://" },
        };

        context.Forms.Add(eventForm);

        // Form 4: Cascading Dropdown Demo
        var cascadingForm = new Form
        {
            Title = "Location Selection Demo",
            Description = "Demonstrates cascading dropdowns - select a country to see its cities.",
            Type = FormType.DataCollection,
            Status = FormStatus.Published,
            Slug = "location-demo",
            CreatedByUserId = adminId,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow
        };

        context.Forms.Add(cascadingForm);
        await context.SaveChangesAsync();

        // Add country field first
        var countryField = new FormField
        {
            FormId = cascadingForm.Id,
            Name = "country",
            Label = "Country",
            FieldType = FieldType.Select,
            IsRequired = true,
            Order = 1,
            Placeholder = "Select a country...",
            OptionsJson = "[{\"value\":\"algeria\",\"text\":\"Algeria\"},{\"value\":\"france\",\"text\":\"France\"},{\"value\":\"usa\",\"text\":\"United States\"}]"
        };
        context.FormFields.Add(countryField);
        await context.SaveChangesAsync();

        // Add city field with parent reference
        var cityField = new FormField
        {
            FormId = cascadingForm.Id,
            Name = "city",
            Label = "City",
            FieldType = FieldType.Select,
            IsRequired = true,
            Order = 2,
            Placeholder = "Select a city...",
            ParentFieldId = countryField.Id,
            OptionsJson = "[{\"value\":\"algiers\",\"text\":\"Algiers\",\"parentValue\":\"algeria\"},{\"value\":\"oran\",\"text\":\"Oran\",\"parentValue\":\"algeria\"},{\"value\":\"setif\",\"text\":\"Sétif\",\"parentValue\":\"algeria\"},{\"value\":\"constantine\",\"text\":\"Constantine\",\"parentValue\":\"algeria\"},{\"value\":\"paris\",\"text\":\"Paris\",\"parentValue\":\"france\"},{\"value\":\"lyon\",\"text\":\"Lyon\",\"parentValue\":\"france\"},{\"value\":\"marseille\",\"text\":\"Marseille\",\"parentValue\":\"france\"},{\"value\":\"newyork\",\"text\":\"New York\",\"parentValue\":\"usa\"},{\"value\":\"losangeles\",\"text\":\"Los Angeles\",\"parentValue\":\"usa\"},{\"value\":\"chicago\",\"text\":\"Chicago\",\"parentValue\":\"usa\"}]"
        };
        context.FormFields.Add(cityField);

        // Add a simple text field
        var nameField = new FormField
        {
            FormId = cascadingForm.Id,
            Name = "full_name",
            Label = "Your Name",
            FieldType = FieldType.Text,
            IsRequired = true,
            Order = 3,
            Placeholder = "Enter your name"
        };
        context.FormFields.Add(nameField);

        await context.SaveChangesAsync();

        // Form 5: Draft form (not published)
        var draftForm = new Form
        {
            Title = "Library Feedback Form",
            Description = "Share your feedback about library services.",
            Type = FormType.Survey,
            Status = FormStatus.Draft,
            Slug = "library-feedback",
            CreatedByUserId = adminId,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            UpdatedAt = DateTime.UtcNow
        };

        draftForm.Fields = new List<FormField>
        {
            new() { Name = "visit_frequency", Label = "How often do you visit the library?", FieldType = FieldType.Select, IsRequired = true, Order = 1, OptionsJson = "[{\"value\":\"daily\",\"text\":\"Daily\"},{\"value\":\"weekly\",\"text\":\"Weekly\"},{\"value\":\"monthly\",\"text\":\"Monthly\"},{\"value\":\"rarely\",\"text\":\"Rarely\"}]" },
            new() { Name = "satisfaction", Label = "Overall Satisfaction", FieldType = FieldType.Radio, IsRequired = true, Order = 2, OptionsJson = "[{\"value\":\"5\",\"text\":\"Very Satisfied\"},{\"value\":\"4\",\"text\":\"Satisfied\"},{\"value\":\"3\",\"text\":\"Neutral\"},{\"value\":\"2\",\"text\":\"Dissatisfied\"},{\"value\":\"1\",\"text\":\"Very Dissatisfied\"}]" },
        };

        context.Forms.Add(draftForm);

        await context.SaveChangesAsync();

        // Now seed sample submissions
        await SeedSampleSubmissionsAsync(context, userManager);
    }

    private static async Task SeedSampleSubmissionsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        var registrationForm = await context.Forms
            .Include(f => f.Fields)
            .FirstOrDefaultAsync(f => f.Slug == "student-registration-2026");

        var surveyForm = await context.Forms
            .Include(f => f.Fields)
            .FirstOrDefaultAsync(f => f.Slug == "course-satisfaction-survey");

        var conferenceForm = await context.Forms
            .Include(f => f.Fields)
            .FirstOrDefaultAsync(f => f.Slug == "conference-2026");

        var student1 = await userManager.FindByEmailAsync("student1@univ-setif.dz");
        var student2 = await userManager.FindByEmailAsync("student2@univ-setif.dz");
        var student3 = await userManager.FindByEmailAsync("student3@univ-setif.dz");

        // Registration form submissions
        if (registrationForm != null)
        {
            var submissions = new[]
            {
                CreateRegistrationSubmission(registrationForm, student1?.Id, "Ahmed", "Benali", "ahmed.benali@email.com", "+213555123456", "1999-03-15", "male", "technology", "m1", "123 Rue Didouche, Sétif"),
                CreateRegistrationSubmission(registrationForm, student2?.Id, "Fatima", "Khelifi", "fatima.khelifi@email.com", "+213666234567", "2000-07-22", "female", "sciences", "l3", "45 Boulevard Zighout, Sétif"),
                CreateRegistrationSubmission(registrationForm, student3?.Id, "Mohamed", "Amrani", "mohamed.amrani@email.com", "+213777345678", "1998-11-08", "male", "economics", "m2", "78 Cité El Hidhab, Sétif"),
                CreateRegistrationSubmission(registrationForm, null, "Sara", "Boudiaf", "sara.boudiaf@email.com", "+213888456789", "2001-01-30", "female", "medicine", "l2", "12 Rue Larbi Ben M'hidi, Sétif"),
                CreateRegistrationSubmission(registrationForm, null, "Youcef", "Mebarki", "youcef.mebarki@email.com", "+213999567890", "1997-09-12", "male", "law", "phd", "56 Avenue de l'ALN, Sétif"),
            };

            foreach (var sub in submissions)
            {
                sub.SubmittedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 25));
                context.FormSubmissions.Add(sub);
            }
        }

        // Survey form submissions
        if (surveyForm != null)
        {
            var submissions = new[]
            {
                CreateSurveySubmission(surveyForm, student1?.Id, "Database Systems", "Dr. Hamid Benslimane", "5", "4", "5", new[] { "content", "instructor", "practical" }, "Excellent course, very practical!", "yes"),
                CreateSurveySubmission(surveyForm, student2?.Id, "Algorithms", "Dr. Nadia Boukhalfa", "4", "5", "4", new[] { "content", "materials" }, "Good content but needs more exercises.", "yes"),
                CreateSurveySubmission(surveyForm, student3?.Id, "Web Development", "Dr. Karim Zidane", "3", "3", "4", new[] { "practical" }, "Need more modern frameworks.", "maybe"),
                CreateSurveySubmission(surveyForm, null, "Machine Learning", "Dr. Amina Ferhat", "5", "5", "5", new[] { "content", "instructor", "materials", "practical" }, "Best course ever!", "yes"),
                CreateSurveySubmission(surveyForm, null, "Statistics", "Dr. Sofiane Belaid", "2", "3", "2", new[] { "materials" }, "Too theoretical.", "no"),
                CreateSurveySubmission(surveyForm, null, "Operating Systems", "Dr. Hamid Benslimane", "4", "4", "4", new[] { "content", "practical" }, "", "yes"),
            };

            foreach (var sub in submissions)
            {
                sub.SubmittedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 15));
                context.FormSubmissions.Add(sub);
            }
        }

        // Conference form submissions
        if (conferenceForm != null)
        {
            var submissions = new[]
            {
                CreateConferenceSubmission(conferenceForm, null, "Dr. John Smith", "john.smith@oxford.edu", "Oxford University", "United Kingdom", "presenter", new[] { "ai", "ml" }, "none", "Deep learning approaches for medical image analysis...", "https://johnsmith.academia.edu"),
                CreateConferenceSubmission(conferenceForm, null, "Prof. Maria Garcia", "m.garcia@upm.es", "Universidad Politécnica de Madrid", "Spain", "presenter", new[] { "iot", "security" }, "vegetarian", "Secure IoT architectures for smart cities...", "https://upm.es/maria-garcia"),
                CreateConferenceSubmission(conferenceForm, null, "Dr. Ahmed Hassan", "a.hassan@cu.edu.eg", "Cairo University", "Egypt", "poster", new[] { "data", "ml" }, "halal", "Predictive analytics in healthcare...", ""),
                CreateConferenceSubmission(conferenceForm, student1?.Id, "Benali Ahmed", "ahmed.benali@univ-setif.dz", "Université Ferhat Abbas Sétif 1", "Algeria", "attendee", new[] { "ai", "data" }, "halal", "", ""),
            };

            foreach (var sub in submissions)
            {
                sub.SubmittedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 8));
                context.FormSubmissions.Add(sub);
            }
        }

        await context.SaveChangesAsync();
    }

    private static FormSubmission CreateRegistrationSubmission(Form form, string? userId, string firstName, string lastName, string email, string phone, string birthDate, string gender, string faculty, string level, string address)
    {
        var submission = new FormSubmission
        {
            FormId = form.Id,
            SubmittedByUserId = userId,
            IpAddress = $"192.168.1.{Random.Shared.Next(1, 255)}",
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
            Status = SubmissionStatus.Completed
        };

        var fields = form.Fields.ToDictionary(f => f.Name, f => f);
        submission.Answers = new List<FormAnswer>
        {
            new() { FieldId = fields["first_name"].Id, ValueText = firstName },
            new() { FieldId = fields["last_name"].Id, ValueText = lastName },
            new() { FieldId = fields["email"].Id, ValueText = email },
            new() { FieldId = fields["phone"].Id, ValueText = phone },
            new() { FieldId = fields["birth_date"].Id, ValueText = birthDate },
            new() { FieldId = fields["gender"].Id, ValueText = gender },
            new() { FieldId = fields["faculty"].Id, ValueText = faculty },
            new() { FieldId = fields["level"].Id, ValueText = level },
            new() { FieldId = fields["address"].Id, ValueText = address },
        };

        return submission;
    }

    private static FormSubmission CreateSurveySubmission(Form form, string? userId, string courseName, string instructor, string overall, string content, string teaching, string[] liked, string comments, string recommend)
    {
        var submission = new FormSubmission
        {
            FormId = form.Id,
            SubmittedByUserId = userId,
            IpAddress = $"192.168.1.{Random.Shared.Next(1, 255)}",
            UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)",
            Status = SubmissionStatus.Completed
        };

        var fields = form.Fields.ToDictionary(f => f.Name, f => f);
        submission.Answers = new List<FormAnswer>
        {
            new() { FieldId = fields["course_name"].Id, ValueText = courseName },
            new() { FieldId = fields["instructor"].Id, ValueText = instructor },
            new() { FieldId = fields["overall_rating"].Id, ValueText = overall },
            new() { FieldId = fields["content_quality"].Id, ValueText = content },
            new() { FieldId = fields["teaching_quality"].Id, ValueText = teaching },
            new() { FieldId = fields["liked_aspects"].Id, ValueJson = System.Text.Json.JsonSerializer.Serialize(liked) },
            new() { FieldId = fields["comments"].Id, ValueText = comments },
            new() { FieldId = fields["recommend"].Id, ValueText = recommend },
        };

        return submission;
    }

    private static FormSubmission CreateConferenceSubmission(Form form, string? userId, string fullName, string email, string institution, string country, string participationType, string[] topics, string dietary, string abstractText, string website)
    {
        var submission = new FormSubmission
        {
            FormId = form.Id,
            SubmittedByUserId = userId,
            IpAddress = $"10.0.0.{Random.Shared.Next(1, 255)}",
            UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36",
            Status = SubmissionStatus.Completed
        };

        var fields = form.Fields.ToDictionary(f => f.Name, f => f);
        submission.Answers = new List<FormAnswer>
        {
            new() { FieldId = fields["full_name"].Id, ValueText = fullName },
            new() { FieldId = fields["email"].Id, ValueText = email },
            new() { FieldId = fields["institution"].Id, ValueText = institution },
            new() { FieldId = fields["country"].Id, ValueText = country },
            new() { FieldId = fields["participation_type"].Id, ValueText = participationType },
            new() { FieldId = fields["topics"].Id, ValueJson = System.Text.Json.JsonSerializer.Serialize(topics) },
            new() { FieldId = fields["dietary"].Id, ValueText = dietary },
            new() { FieldId = fields["abstract"].Id, ValueText = abstractText },
            new() { FieldId = fields["website"].Id, ValueText = website },
        };

        return submission;
    }
}
