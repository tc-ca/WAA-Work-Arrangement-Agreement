using Data;
using GoC.TC.SecureMailer;
using GoC.TC.SecureMailer.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Serilog;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Utilities;
using Utilities.Middleware;
using Web.api;
using Web.Classes;
using Web.Localization;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        private readonly IWebHostEnvironment _webHostEnvironment;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register instance of Configuration, primarily to allow the Utilities library to access appsettings.
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton(Log.Logger);
            services.AddSingleton<IValidationAttributeAdapterProvider, CustomValidationAttributeAdapterProvider>();

            services.AddScoped<AgreementService>();
            //services.AddScoped<AddressService>();
            //services.AddScoped<NeighbourhoodService>();
            services.AddScoped<EmployeeService>();
            services.AddScoped<Notifications>();
           // services.AddScoped<Utils>();
            services.AddScoped<RazorPartialToStringRenderer>();
            services.AddTransient<LocalizedRouteMap>();
            services.AddScoped<SecureSmtpClient>(x => new SecureSmtpClient(new DefaultConfigStrategy(true))
            //requester: Configuration["AppSettings:AppAcronymEN"],
            //emailServiceURL: Configuration["EmailSettings:URL"],
            //username: Configuration["EmailSettings:Username"],
            //password: Configuration["EmailSettings:Password"]
            );
            services.AddHttpClient();
            services.Configure<CanadaPostApiSetting>( Configuration.GetSection("AppSettings:CanadaPostApi"));
            services.AddHttpContextAccessor();
            services.AddDbContext<DomainContext>(options =>
               //options.UseOracle(Configuration.GetConnectionString("TipsDb"))
               options.UseOracle(new Utils(Configuration, _webHostEnvironment).GetConnectionString())
               .AddInterceptors(new OracleCommandInterceptor())
           ) ;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMemoryCache();

            // Cookie.IsEssential is REQUIRED in order to persist the Session over multiple requests.
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.IsEssential = true;
            });
            services.Configure<CookieTempDataProviderOptions>(options => {
                options.Cookie.IsEssential = true;
            });
            // Enables authentication. It works but mostly due to trial and error so this may
            // be able to be simplified or improved.
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => options.AccessDeniedPath = "/en/Unauthorized");

            // Adds authorization and registers policies.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.Requirements.Add(new PermissionRequirement("Admin")));
                // Register additional policies here.
            });

            services.AddScoped<IAuthorizationHandler, UserPermissionMiddleware>();

            services.AddLocalization(options => options.ResourcesPath = "PageResources");

            // Configures cultures for localization.
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new[]
                {
                        new CultureInfo("en"),
                        new CultureInfo("fr")
                    };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
                options.RequestCultureProviders.Insert(0, new RouteValueRequestCultureProvider(cultures));
            });

            _ = services.AddMvc(options =>
              {
                  options.EnableEndpointRouting = false;
              })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization()
            .AddRazorOptions(o =>
            {
                o.PageViewLocationFormats.Add("/Pages/Shared/{0}/{0}.cshtml");
                o.PageViewLocationFormats.Add("/Pages/Shared/{0}.cshtml");
                o.PageViewLocationFormats.Add("/Pages/Admin/Shared/{0}.cshtml");
            })
            .AddRazorPagesOptions(o =>
            {
                var sp = services.BuildServiceProvider();
                o.Conventions.Add(new LocalizedRouteModelConvention(sp.GetService<LocalizedRouteMap>()));
            })
            .AddJsonOptions(o => { o.JsonSerializerOptions.PropertyNamingPolicy = null; o.JsonSerializerOptions.PropertyNamingPolicy = null; });
            
        
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Configuration["AppSettings:KeyRepo"]));
            
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, SecureSmtpClient emailClient)
        {
            app.UseHttpsRedirection();
          
            // app.UseStaticFiles();
            // force IIS to set content type as UTF8 for CSS files            
            var extensionProvider = new FileExtensionContentTypeProvider();
            extensionProvider.Mappings[".css"] = "text/css; charset=utf-8";
            
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = extensionProvider
            });
            app.UseCookiePolicy();
            app.UseRequestLocalization();

            RewriteOptions rewriter = new RewriteOptions();
            //rewriter.AddRewrite(@"^IWA/en/(.*)", "en/$1", skipRemainingRules: false);
            rewriter.Add(new RedirectLocalizedRoutes(app.ApplicationServices.GetService<LocalizedRouteMap>()));
            
            app.UseRewriter(rewriter);
            //app.UsePathBase(new PathString("/IWA"));

            app.UseSerilogRequestLogging();
            app.UseAuthentication();
            app.UseSession();


            //pipeline ordering
            if (env.IsDevelopment() || Boolean.Parse(config["AppSettings:ForceDeveloperExceptionPage"]))
            {
                 app.UseDeveloperExceptionPage();               
            }
            else
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = new ExceptionEmailerMiddleware(config).Invoke
                });
              //  app.UseExceptionHandler("/en/Error");
            }
            app.UseMiddleware<UserMiddleware>();

            app.UseMvc();

        }
    }
}
