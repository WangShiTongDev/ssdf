//using DevTeamUp.BLL.AutoMapper;
using DevTeamUp.BLL.AutoMapper;
using DevTeamUp.BLL.Services;
using DevTeamUp.DAL.EF;
using DevTeamUp.DAL.EF.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;


namespace DevTeamUp
{

    /*
     * ����:
     * ���������� ��� ����� ����������� ������������ ����� ��������� 
     * 
     * ����� ����� ����� �������� �������� UserProfile ��� ����������� ������� �����
     */


    /* ��������� ��� ��������:
     * 
     * EF: ��� �������������� �������, ���� ����� �������, ������� � ��� ��� EF �� �������� ��� 
     * ������������ ���. �������� � ���������� ���, ��� ��� ������� �� ���������� � ��������� 
     * �� ����� ������� ��, ��� ��� ��� ����. �����������.
     * � � ����������� ������ �������� NoAction, �� ��� ����� ����������� � �������� � ����������� �� ������ ������
     * 
     * 
     * 
     * 
     */

    
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews( config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddRazorRuntimeCompilation();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(op =>
                {
                    //TODO: write login path
                    op.LogoutPath = new PathString("/Account/Login");
                });

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1; // ����������� ����� ������
                options.Password.RequiredUniqueChars = 0; // ����������� ���������� ���������� �������� � ������
            });

            builder.Services.AddAuthorization();
            builder.Services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();


            // DB
            builder.Services.AddDbContext<DataContext>(op =>
                op.UseSqlServer(builder.Configuration.GetConnectionString("cs1"))
            );

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // services
            builder.Services.AddTransient(typeof(ProjectService));
            builder.Services.AddTransient(typeof(SkillService));
            builder.Services.AddTransient(typeof(UserService));


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