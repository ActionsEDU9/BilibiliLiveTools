using System;
using System.Threading;
using System.Threading.Tasks;
using Bilibili.AspNetCore.Apis.DependencyInjection;
using BilibiliAutoLiver.Config;
using BilibiliAutoLiver.DependencyInjection;
using BilibiliAutoLiver.Plugin.Base;
using BilibiliAutoLiver.Services;
using BilibiliAutoLiver.Services.FFMpeg.Services.PushService;
using BilibiliAutoLiver.Services.Interface;
using BilibiliAutoLiver.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace BilibiliAutoLiver
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = $"������������ֵ��ֱ������ v{VersionHelper.GetVersion()} By withsalt(https://github.com/withsalt)";

            var builder = WebApplication.CreateBuilder(args);

            //Add NLog
            builder.Logging.ClearProviders();
            builder.Logging.AddNLogWeb();

            //���ó�ʼ��
            builder.Services.ConfigureSettings(builder);

            //Db
            builder.Services.AddDatabase();
            builder.Services.AddRepository();

            //����
            builder.Services.AddMemoryCache();
            //���Bilibili��صķ���
            builder.Services.AddBilibiliApis(options =>
            {
                builder.Configuration.GetSection("AppSettings:BilibiliAppKey").Bind(options);
            }, false);
            //��ʱ����
            builder.Services.AddQuartz();
            //FFMpeg
            builder.Services.AddFFmpegService();
            //���
            builder.Services.AddPipePlugins();
            //�ʼ�����
            builder.Services.AddTransient<IEmailNoticeService, EmailNoticeService>();

            builder.Services.AddSingleton<INormalPushStreamService, NormalPushStreamService>();
            builder.Services.AddSingleton<IAdvancePushStreamService, AdvancePushStreamService>();
            builder.Services.AddSingleton<IPushStreamProxyService, PushStreamProxyService>();
            builder.Services.AddTransient<IStartupService, StartupService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(GlobalConfigConstant.DEFAULT_ORIGINS_NAME, policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromDays(3650 * 10);
                    options.SlidingExpiration = true;
                    options.AccessDeniedPath = "/Account/Login";
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.ReturnUrlParameter = "";
                });

            //ʹ��һ��Ĭ�ϲ���
            var urls = builder.Configuration["ASPNETCORE_URLS"] ?? builder.Configuration["urls"];
            if (string.IsNullOrWhiteSpace(urls))
            {
                builder.WebHost.UseUrls("http://*:18686");
            }

#if DEBUG
            builder.Services.AddControllersWithViews()
                .AddJsonConfig()
                .AddRazorRuntimeCompilation();
#else
            builder.Services.AddControllersWithViews()
                .AddJsonConfig();
#endif

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //���ݷ������
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //����
            app.UseCors(GlobalConfigConstant.DEFAULT_ORIGINS_NAME);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMediaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //��ʼ�����ݿ�
            app.InitializeDatabase();

            app.Lifetime.ApplicationStarted.Register((obj, token)
                => Task.Run(() => app.Services.GetRequiredService<IStartupService>().Start(token), CancellationToken.None), null);

            await app.RunAsync();
        }
    }
}
