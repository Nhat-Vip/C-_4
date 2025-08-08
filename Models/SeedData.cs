using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context =
            serviceProvider.GetRequiredService<MyDbContext>()
        )
        {
            if (context == null || context.Users == null)
            {
                throw new ArgumentNullException("Null RazorPagesMovieContext");
            }
            if (context.Users.Any())
            {
                return;
            }
            context.Users.AddRange(
                new User
                {
                    UserName = "Admin",
                    Email = "admin@gmail.com",
                    Role = Role.Admin,
                    PhoneNumber = "09998885432",
                    PassWord = "Admin@123"
                }
            );
            context.SaveChanges();
        }
    }
}