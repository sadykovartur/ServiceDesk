using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceDesk.API.Models;

namespace ServiceDesk.API.Data;

public static class DbInitializer
{
    private record SeedUser(string Email, string Password, string DisplayName, string Role);

    private static readonly SeedUser[] Users =
    [
        new("admin@demo.com",     "Admin123!",    "Admin",         "Admin"),
        new("operator1@demo.com", "Operator123!", "Operator One",  "Operator"),
        new("operator2@demo.com", "Operator123!", "Operator Two",  "Operator"),
        new("student1@demo.com",  "Student123!",  "Student One",   "Student"),
        new("student2@demo.com",  "Student123!",  "Student Two",   "Student"),
        new("student3@demo.com",  "Student123!",  "Student Three", "Student"),
    ];

    private static readonly (string Name, bool IsActive)[] Categories =
    [
        ("Hardware Issues",     true),
        ("Software Issues",     true),
        ("Network Problems",    true),
        ("Account Management",  true),
        ("Security Concerns",   false),
    ];

    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var db          = services.GetRequiredService<AppDbContext>();
        var logger      = services.GetRequiredService<ILogger<AppDbContext>>();

        await SeedUsersAsync(userManager, logger);
        await SeedCategoriesAsync(db, logger);
        await SeedTicketsAsync(db, userManager, logger);
    }

    // ── Users ────────────────────────────────────────────────────────────────

    private static async Task SeedUsersAsync(
        UserManager<ApplicationUser> userManager,
        ILogger logger)
    {
        foreach (var seed in Users)
        {
            var user = await userManager.FindByEmailAsync(seed.Email);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName    = seed.Email,
                    Email       = seed.Email,
                    DisplayName = seed.DisplayName
                };

                var result = await userManager.CreateAsync(user, seed.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogError("Failed to seed user {Email}: {Errors}", seed.Email, errors);
                    continue;
                }
            }

            if (!await userManager.IsInRoleAsync(user, seed.Role))
            {
                var roleResult = await userManager.AddToRoleAsync(user, seed.Role);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    logger.LogError("Failed to assign role {Role} to {Email}: {Errors}", seed.Role, seed.Email, errors);
                    continue;
                }

                logger.LogInformation("Assigned role {Role} to {Email}", seed.Role, seed.Email);
            }

            logger.LogInformation("Seeded user {Email} with role {Role}", seed.Email, seed.Role);
        }
    }

    // ── Categories ───────────────────────────────────────────────────────────

    private static async Task SeedCategoriesAsync(AppDbContext db, ILogger logger)
    {
        foreach (var (name, isActive) in Categories)
        {
            var exists = await db.Categories.AnyAsync(c => c.Name == name);
            if (exists) continue;

            db.Categories.Add(new Category { Name = name, IsActive = isActive });
            logger.LogInformation("Seeding category '{Name}' (active={IsActive})", name, isActive);
        }

        await db.SaveChangesAsync();
    }

    // ── Tickets ──────────────────────────────────────────────────────────────

    private static async Task SeedTicketsAsync(
        AppDbContext db,
        UserManager<ApplicationUser> userManager,
        ILogger logger)
    {
        if (await db.Tickets.AnyAsync())
        {
            logger.LogInformation("Tickets already seeded — skipping.");
            return;
        }

        // Resolve user IDs by email
        var s1 = (await userManager.FindByEmailAsync("student1@demo.com"))!;
        var s2 = (await userManager.FindByEmailAsync("student2@demo.com"))!;
        var s3 = (await userManager.FindByEmailAsync("student3@demo.com"))!;
        var op1 = (await userManager.FindByEmailAsync("operator1@demo.com"))!;
        var op2 = (await userManager.FindByEmailAsync("operator2@demo.com"))!;

        // Resolve category IDs by name (already seeded above)
        var catHardware  = await db.Categories.FirstAsync(c => c.Name == "Hardware Issues");
        var catSoftware  = await db.Categories.FirstAsync(c => c.Name == "Software Issues");
        var catNetwork   = await db.Categories.FirstAsync(c => c.Name == "Network Problems");
        var catAccount   = await db.Categories.FirstAsync(c => c.Name == "Account Management");
        var catSecurity  = await db.Categories.FirstAsync(c => c.Name == "Security Concerns");

        var now = DateTimeOffset.UtcNow;

        var tickets = new List<Ticket>
        {
            // ── New ───────────────────────────────────────────────────────────
            new()
            {
                Title       = "Keyboard stopped working after reboot",
                Description = "USB keyboard is not recognized after the latest system restart.",
                Priority    = TicketPriority.Low,
                Status      = TicketStatus.New,
                AuthorId    = s1.Id,
                CategoryId  = catHardware.Id,
                CreatedAt   = now.AddDays(-10),
                UpdatedAt   = now.AddDays(-10)
            },
            new()
            {
                Title       = "Excel crashes when opening large files",
                Description = "Microsoft Excel closes unexpectedly for files larger than 50 MB.",
                Priority    = TicketPriority.Medium,
                Status      = TicketStatus.New,
                AuthorId    = s2.Id,
                CategoryId  = catSoftware.Id,
                CreatedAt   = now.AddDays(-9),
                UpdatedAt   = now.AddDays(-9)
            },
            new()
            {
                Title       = "No internet access in Lab 3",
                Description = "All workstations in Lab 3 lost internet connectivity since this morning.",
                Priority    = TicketPriority.High,
                Status      = TicketStatus.New,
                AuthorId    = s3.Id,
                CategoryId  = catNetwork.Id,
                CreatedAt   = now.AddDays(-8),
                UpdatedAt   = now.AddDays(-8)
            },

            // ── InProgress ────────────────────────────────────────────────────
            new()
            {
                Title       = "Monitor flickering at 60 Hz",
                Description = "The Dell monitor in room 204 flickers when refresh rate is set to 60 Hz.",
                Priority    = TicketPriority.Low,
                Status      = TicketStatus.InProgress,
                AuthorId    = s1.Id,
                AssigneeId  = op1.Id,
                CategoryId  = catHardware.Id,
                CreatedAt   = now.AddDays(-7),
                UpdatedAt   = now.AddDays(-6)
            },
            new()
            {
                Title       = "VPN client fails to connect",
                Description = "Cisco AnyConnect throws 'Connection timed out' from off-campus.",
                Priority    = TicketPriority.Medium,
                Status      = TicketStatus.InProgress,
                AuthorId    = s2.Id,
                AssigneeId  = op1.Id,
                CategoryId  = catNetwork.Id,
                CreatedAt   = now.AddDays(-6),
                UpdatedAt   = now.AddDays(-5)
            },
            new()
            {
                Title       = "Cannot reset forgotten password",
                Description = "Password reset email is not arriving despite multiple attempts.",
                Priority    = TicketPriority.High,
                Status      = TicketStatus.InProgress,
                AuthorId    = s3.Id,
                AssigneeId  = op2.Id,
                CategoryId  = catAccount.Id,
                CreatedAt   = now.AddDays(-5),
                UpdatedAt   = now.AddDays(-4)
            },

            // ── WaitingForStudent ─────────────────────────────────────────────
            new()
            {
                Title       = "Printer not found on the network",
                Description = "HP LaserJet in the library is not discoverable from any workstation.",
                Priority    = TicketPriority.Low,
                Status      = TicketStatus.WaitingForStudent,
                AuthorId    = s1.Id,
                AssigneeId  = op1.Id,
                CategoryId  = catNetwork.Id,
                CreatedAt   = now.AddDays(-12),
                UpdatedAt   = now.AddDays(-3)
            },
            new()
            {
                Title       = "Blue screen of death on startup",
                Description = "Machine shows BSOD (IRQL_NOT_LESS_OR_EQUAL) on boot.",
                Priority    = TicketPriority.High,
                Status      = TicketStatus.WaitingForStudent,
                AuthorId    = s2.Id,
                AssigneeId  = op2.Id,
                CategoryId  = catHardware.Id,
                CreatedAt   = now.AddDays(-11),
                UpdatedAt   = now.AddDays(-2)
            },

            // ── Resolved ──────────────────────────────────────────────────────
            new()
            {
                Title       = "Antivirus definitions out of date",
                Description = "Avast antivirus on workstation #12 shows definitions from 3 months ago.",
                Priority    = TicketPriority.Medium,
                Status      = TicketStatus.Resolved,
                AuthorId    = s3.Id,
                AssigneeId  = op1.Id,
                CategoryId  = catSoftware.Id,
                CreatedAt   = now.AddDays(-20),
                UpdatedAt   = now.AddDays(-1)
            },
            new()
            {
                Title       = "Student portal login loop",
                Description = "After entering credentials the portal redirects back to the login page.",
                Priority    = TicketPriority.High,
                Status      = TicketStatus.Resolved,
                AuthorId    = s1.Id,
                AssigneeId  = op2.Id,
                CategoryId  = catAccount.Id,
                CreatedAt   = now.AddDays(-15),
                UpdatedAt   = now.AddDays(-1)
            },

            // ── Closed ────────────────────────────────────────────────────────
            new()
            {
                Title       = "Wi-Fi drops every 30 minutes",
                Description = "Wireless connection disconnects periodically in building B.",
                Priority    = TicketPriority.Medium,
                Status      = TicketStatus.Closed,
                AuthorId    = s2.Id,
                AssigneeId  = op1.Id,
                CategoryId  = catNetwork.Id,
                CreatedAt   = now.AddDays(-30),
                UpdatedAt   = now.AddDays(-5)
            },
            new()
            {
                Title       = "RAM upgrade request for lab workstations",
                Description = "Workstations in Lab 1 need RAM upgraded from 4 GB to 8 GB.",
                Priority    = TicketPriority.Low,
                Status      = TicketStatus.Closed,
                AuthorId    = s3.Id,
                AssigneeId  = op2.Id,
                CategoryId  = catHardware.Id,
                CreatedAt   = now.AddDays(-25),
                UpdatedAt   = now.AddDays(-7)
            },
            new()
            {
                Title       = "Cannot install Python 3.11",
                Description = "Installer exits with error code 1603 on Windows 10 Education.",
                Priority    = TicketPriority.Medium,
                Status      = TicketStatus.Closed,
                AuthorId    = s1.Id,
                AssigneeId  = op1.Id,
                CategoryId  = catSoftware.Id,
                CreatedAt   = now.AddDays(-22),
                UpdatedAt   = now.AddDays(-10)
            },

            // ── Rejected ──────────────────────────────────────────────────────
            new()
            {
                Title           = "Request for personal cloud storage account",
                Description     = "Please provide a 1 TB personal OneDrive account for course work.",
                Priority        = TicketPriority.Low,
                Status          = TicketStatus.Rejected,
                AuthorId        = s1.Id,
                AssigneeId      = op1.Id,
                RejectedReason  = "Personal cloud storage provisioning is outside the IT support scope. Use the shared institutional storage instead.",
                CategoryId      = catAccount.Id,
                CreatedAt       = now.AddDays(-18),
                UpdatedAt       = now.AddDays(-15)
            },
            new()
            {
                Title           = "Install unlicensed Photoshop on lab PC",
                Description     = "Need Adobe Photoshop installed on workstation #7 for my project.",
                Priority        = TicketPriority.Medium,
                Status          = TicketStatus.Rejected,
                AuthorId        = s2.Id,
                AssigneeId      = op2.Id,
                RejectedReason  = "Unlicensed software cannot be installed per the institution's software policy. Please request access through the official Adobe licensing agreement.",
                CategoryId      = catSecurity.Id,
                CreatedAt       = now.AddDays(-14),
                UpdatedAt       = now.AddDays(-12)
            },
            new()
            {
                Title           = "Disable firewall on workstation",
                Description     = "The firewall is blocking my game launcher; please disable it.",
                Priority        = TicketPriority.High,
                Status          = TicketStatus.Rejected,
                AuthorId        = s3.Id,
                AssigneeId      = op1.Id,
                RejectedReason  = "Disabling the firewall violates security policy. Consider requesting a specific firewall rule exception for the required application ports.",
                CategoryId      = catSecurity.Id,
                CreatedAt       = now.AddDays(-13),
                UpdatedAt       = now.AddDays(-11)
            },
        };

        db.Tickets.AddRange(tickets);
        await db.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} tickets.", tickets.Count);
    }
}
