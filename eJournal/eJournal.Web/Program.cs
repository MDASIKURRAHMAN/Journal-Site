using eJournal.Domain.ApplicationContext;
using eJournal.Repository;
using eJournal.Services.Implementions;
using eJournal.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace eJournal.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<EJournalDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("EJournalConnectionString")));	

            // Add the Application related services here.
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBlogService, BlogService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<ILikeService, LikeService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<ILikeService, LikeService>();
            // Adding Authentication Services

            var googleOAuthOptions = builder.Configuration.GetSection("GoogleOAuth");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme=CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath= "/account/Login";
            })
            .AddGoogle(options =>
            {

                options.ClientId = googleOAuthOptions.GetValue<string>("ClientId");
                options.ClientSecret = googleOAuthOptions.GetValue<string>("ClientSecret");
                options.ClaimActions.MapJsonKey("Image_url", "picture");
                options.ClaimActions.MapJsonKey("username", "preferred_username");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();

        }
    }
}
