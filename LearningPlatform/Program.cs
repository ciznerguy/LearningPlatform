using LearningClassLibrary.Services;
using LearningPlatform.Components;
namespace LearningPlatform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // הוספת LoginSession לשירותים
            builder.Services.AddSingleton<LoginSession>();

            // Register HttpClient עם כותרת מותאמת אישית
            builder.Services.AddScoped(sp =>
            {
                var loginSession = sp.GetRequiredService<LoginSession>();
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri("https://localhost:7160/")
                };

                // הוספת הכותרת "User-Role" לכל בקשה
                httpClient.DefaultRequestHeaders.Add("User-Role", loginSession.Role);
                return httpClient;
            });


            // הוספת שירותי Controllers
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            // הוספת Middleware עבור Authorization
            app.UseRouting();
            app.UseAuthentication(); // אם יש לך Authentication
            app.UseAuthorization(); // קריטי להפעיל את המדיניות

            app.UseAntiforgery();

            // רישום נתיבים עבור API
            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
