using Microsoft.AspNetCore.Identity;
using Surefire.Domain.Forms.Models;
using Surefire.Domain.Shared.Models;
using Surefire.Domain.Attachments.Models;

namespace Surefire.Data
{
    public static class SeedInitialData
    {
        public static void SeedData(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Check if the admin user exists
            if (!userManager.Users.Any())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "demo@surefire.com",
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "demo@surefire.com",
                    PhoneNumber = "3103103103",
                    PictureUrl = "default.png",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(adminUser, "Password123!").Result;

                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create the admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Seed Folders
            if (!context.Folders.Any())
            {
                var folders = new[]
                {
                    new Folder { Name = "Policy", Description = "For declarations pages and copies of policies" },
                    new Folder { Name = "Endorsement", Description = "For carrier endorsements and policy changes" },
                    new Folder { Name = "Quote", Description = "For quotes and documents from carriers regarding renewals and new business" },
                    new Folder { Name = "Accounting", Description = "Invoices and bills and what not" },
                    new Folder { Name = "Application", Description = "Apps and supps and supps and apps" },
                    new Folder { Name = "Claims", Description = "Loss related docs like claims, loss runs, etc" }
                };
                context.Folders.AddRange(folders);
            }

            // Seed Settings
            if (!context.Settings.Any())
            {
                var settings = new Surefire.Domain.Shared.Models.Settings
                {
                    FileStore = FileStoreType.Local,
                    DisablePlugins = false
                };
                context.Settings.Add(settings);
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var products = new[]
                {
                    new Product { LineName = "Worker's Compensation", Description = "Commercial", LineCode = "WCO", LineNickname = "Work Comp" },
                    new Product { LineName = "General Liability", Description = "Commercial", LineCode = "GLI", LineNickname = "Gen Liability" },
                    new Product { LineName = "Commercial Auto", Description = "Commercial", LineCode = "AUT", LineNickname = "Com Auto" },
                    new Product { LineName = "Professional Liability", Description = "Commercial", LineCode = "E&O", LineNickname = "Prof Liability" },
                    new Product { LineName = "Business Owner's Package", Description = "Commercial", LineCode = "BOP", LineNickname = "BOP" },
                    new Product { LineName = "Excess and Umbrella", Description = "Commercial", LineCode = "UMB", LineNickname = "Umbrella" },
                    new Product { LineName = "Eployer's Practice Liability", Description = "Commercial", LineCode = "EPL", LineNickname = "EPLI" },
                    new Product { LineName = "Property", Description = "Commercial", LineCode = "PRP", LineNickname = "Property" },
                    new Product { LineName = "Directors and Officers", Description = "Commercial", LineCode = "D&O", LineNickname = "DOLI" },
                    new Product { LineName = "Cyber Liability", Description = "Commercial", LineCode = "CYB", LineNickname = "Cyber" },
                    new Product { LineName = "Group Medical", Description = "Medical", LineCode = "MED", LineNickname = "Grp Med" },
                    new Product { LineName = "Group Dental", Description = "Medical", LineCode = "DEN", LineNickname = "Grp Dental" },
                    new Product { LineName = "Personal Home", Description = "Personal", LineCode = "HOME", LineNickname = "Homeowners" },
                    new Product { LineName = "Personal Auto", Description = "Personal", LineCode = "AUTO", LineNickname = "Personal Auto" },
                    new Product { LineName = "Life", Description = "Personal", LineCode = "LIFE", LineNickname = "Life Insurance" }
                };
                context.Products.AddRange(products);
            }

            // Seed FormPdfs
            if (!context.FormPdf.Any())
            {
                var formPdfs = new[]
                {
                    new FormPdf { Title = "Acord 125 (2016/03)", Description = "Commercial Insurance Application", Filepath = "a125-2016-03.pdf", DateCreated = DateTime.Now, DateModified = DateTime.Now },
                    new FormPdf { Title = "Acord 126 (2016/09)", Description = "Commercial General Liability Section", Filepath = "a126-2016-09.pdf", DateCreated = DateTime.Now, DateModified = DateTime.Now },
                    new FormPdf { Title = "Acord 130 (2017/05)", Description = "Workers Compensation Application", Filepath = "a130-2017-05.pdf", DateCreated = DateTime.Now, DateModified = DateTime.Now },
                    new FormPdf { Title = "Acord 140 (2016/03)", Description = "Property Section", Filepath = "a140-2016-03.pdf", DateCreated = DateTime.Now, DateModified = DateTime.Now },
                    new FormPdf { Title = "Acord 131 (2017/11)", Description = "Umbrella / Excess Section", Filepath = "a131-2017-11.pdf", DateCreated = DateTime.Now, DateModified = DateTime.Now },
                    new FormPdf { Title = "Acord 127 (2012/03)", Description = "Business Auto", Filepath = "a127-2012-03.pdf", DateCreated = DateTime.Now, DateModified = DateTime.Now },
                    new FormPdf { Title = "Acord 80 (2013/09)", Description = "Homeowners Application", Filepath = "a080-2013-09.pdf", DateCreated = DateTime.Now, DateModified = DateTime.Now },
                    new FormPdf { Title = "SL-2 (2024/01)", Description = "Diligent Search Report", Filepath = "sl-2-2024-01.pdf", DateCreated = DateTime.Now, DateModified = DateTime.Now },
                    new FormPdf { Title = "Accounting Sheet", Description = "Accounting Sheet and Trust Check Request", Filepath = "sf-trustcheck-req.pdf", DateCreated = DateTime.Now, DateModified = DateTime.Now }
                };
                context.FormPdf.AddRange(formPdfs);
            }

            // Save changes to the database
            context.SaveChanges();
        }
    }
}
