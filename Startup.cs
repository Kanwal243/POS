using EyeHospitalPOS.Data;
using EyeHospitalPOS.Interfaces;
using EyeHospitalPOS.Models;
using EyeHospitalPOS.Services;
using EyeHospitalPOS.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Security.Claims;

namespace EyeHospitalPOS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            
            // DATABASE
           
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

           
            // BLAZOR
         
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddControllers();
            services.AddHttpContextAccessor();

           
            // AUTHENTICATION
            // UI → Cookies
            // API → JWT
          
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            })
            .AddJwtBearer("Jwt", options =>
            {
                var key = Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"]);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization();

            // ============================
            // BLAZOR AUTH STATE
            // ============================
            // services.AddScoped<CustomAuthenticationStateProvider>();
            // services.AddScoped<AuthenticationStateProvider>(sp =>
            //     sp.GetRequiredService<CustomAuthenticationStateProvider>());

            // ============================
            // DEVEXPRESS
            // ============================
            services.AddDevExpressBlazor(config =>
            {
                config.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
            });

            // ============================
            // APPLICATION SERVICES
            // ============================
            services.AddScoped<Helper.LoginManager>();
            services.AddScoped<IJwtService, JwtService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISalesService, SalesService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBarcodeService, BarcodeService>();
            
            // Register Controllers manually for Blazor Injection
            services.AddScoped<EyeHospitalPOS.Controllers.LoginController>();
            services.AddScoped<EyeHospitalPOS.Controllers.DashboardController>();
            services.AddScoped<EyeHospitalPOS.Controllers.POSController>();
            services.AddScoped<EyeHospitalPOS.Controllers.ProductController>();
            services.AddScoped<EyeHospitalPOS.Controllers.UserController>();

            // Session Support
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            });

            // ============================
            // SWAGGER (API ONLY)
            // ============================
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "EyeHospitalPOS API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        // ============================
        // CONFIGURE PIPELINE
        // ============================
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();

            // Auto-Login Middleware: Hydrate User from Session
            app.Use(async (context, next) =>
            {
                var loginManager = context.RequestServices.GetRequiredService<Helper.LoginManager>();
                await loginManager.InitializeAsync();
                
                if (loginManager.CurrentUser != null)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, loginManager.CurrentUser.UserName),
                        new Claim(ClaimTypes.Email, loginManager.CurrentUser.Email ?? ""),
                        new Claim(ClaimTypes.Role, loginManager.CurrentUser.Role?.Name ?? "User"),
                        new Claim("UserId", loginManager.CurrentUser.Id.ToString()),
                        new Claim("IsSessionBased", "true")
                    };
                    var identity = new ClaimsIdentity(claims, "SessionAuth");
                    context.User = new ClaimsPrincipal(identity);
                }
                
                await next();
            });
            app.UseAuthorization();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EyeHospitalPOS API v1");
                });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();     // API
                endpoints.MapBlazorHub();       // Blazor SignalR
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}



//using EyeHospitalPOS.Middleware;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.HttpsPolicy;
//*//*using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI;*//*
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.OpenApi;
//using Microsoft.OpenApi.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.JSInterop;

//namespace EyeHospitalPOS
//{
//    public class Startup
//    {
//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }

//        public IConfiguration Configuration { get; }

//        public void ConfigureServices(IServiceCollection services)
//        {
//            // DbContext
//            services.AddDbContext<ApplicationDbContext>(options =>
//                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

//            // Blazor services
//            services.AddRazorPages();
//            services.AddControllers(options =>
//            {
//                options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
//            }).AddJsonOptions(options =>
//            {
//                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
//            });
//            services.AddServerSideBlazor();
//            // Configure HttpClient for API calls with IHttpContextAccessor support
//            services.AddHttpClient("ApiClient", (sp, client) =>
//            {
//                var httpContextAccessor = sp.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
//                var request = httpContextAccessor.HttpContext?.Request;

//                client.BaseAddress = request != null 
//                    ? new Uri($"{request.Scheme}://{request.Host}")
//                    : new Uri(Configuration["ApiBaseUrl"] ?? "https://localhost:7146/");

//                client.Timeout = TimeSpan.FromSeconds(30);
//                client.DefaultRequestHeaders.Add("Accept", "application/json");
//            })
//            .ConfigurePrimaryHttpMessageHandler(() => new System.Net.Http.HttpClientHandler
//            {
//                UseCookies = true
//            });

//            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));
//            services.AddDevExpressBlazor(configure => configure.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5);

//            // Session Services
//            services.AddDistributedMemoryCache();
//            services.AddSession(options =>
//            {
//                options.IdleTimeout = TimeSpan.FromMinutes(30);
//                options.Cookie.HttpOnly = true;
//                options.Cookie.IsEssential = true;
//                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
//            });
//            services.AddHttpContextAccessor();
//            services.AddScoped<IHttpContextService, HttpContextService>();

//            // Custom AuthenticationStateProvider using LoginManager
//            services.AddScoped<CustomAuthenticationStateProvider>();
//            services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomAuthenticationStateProvider>());

//            // Developer exception page
//            services.AddDatabaseDeveloperPageExceptionFilter();

//            // Register LoginManager
//            services.AddScoped<EyeHospitalPOS.Helper.LoginManager>();

//            // Server-side services for API controllers
//            services.AddScoped<IAuthService, AuthService>();
//            services.AddScoped<IProductService, ProductService>();
//            services.AddScoped<ISalesService, SalesService>();
//            services.AddScoped<IInventoryService, InventoryService>();
//            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
//            services.AddScoped<IUserService, UserService>();
//            services.AddScoped<IBarcodeService, BarcodeService>();

//            // API client services for Blazor pages (use HttpClient)
//            services.AddScoped<ApiClientService>();
//            services.AddScoped<ApiInventoryService>(sp => new ApiInventoryService(
//                sp.GetRequiredService<HttpClient>(),
//                sp.GetRequiredService<IJSRuntime>(),
//                sp.GetRequiredService<IHttpContextService>(),
//                sp.GetRequiredService<Helper.LoginManager>()));
//            services.AddScoped<ApiSalesService>(sp => new ApiSalesService(
//                sp.GetRequiredService<HttpClient>(),
//                sp.GetRequiredService<IJSRuntime>(),
//                sp.GetRequiredService<IHttpContextService>(),
//                sp.GetRequiredService<Helper.LoginManager>()));
//            services.AddScoped<ApiAuthService>(sp => new ApiAuthService(
//                sp.GetRequiredService<HttpClient>(),
//                sp.GetRequiredService<IJSRuntime>(),
//                sp.GetRequiredService<IHttpContextService>(),
//                sp.GetRequiredService<Helper.LoginManager>()));

//            // Register Controllers
//            services.AddScoped<EyeHospitalPOS.Controllers.LoginController>();
//            services.AddScoped<EyeHospitalPOS.Controllers.DashboardController>();
//            services.AddScoped<EyeHospitalPOS.Controllers.ProductController>();
//            services.AddScoped<EyeHospitalPOS.Controllers.POSController>();
//            services.AddScoped<EyeHospitalPOS.Controllers.UserController>();

//            // Register JWT Service
//            services.AddScoped<IJwtService, JwtService>();

//            // Antiforgery
//            services.AddAntiforgery(options =>
//            {
//                options.HeaderName = "X-XSRF-TOKEN";
//                options.Cookie.Name = "XSRF-TOKEN";
//                options.Cookie.HttpOnly = false; // Accessible by JS for sending in headers
//                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
//            });

//            // JWT Authentication
//            services.AddAuthentication(options =>
//            {
//                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//            }).AddJwtBearer(options =>
//            {
//                var secretKey = Configuration["Jwt:SecretKey"];
//                var issuer = Configuration["Jwt:Issuer"];
//                var audience = Configuration["Jwt:Audience"];

//                // Validate JWT configuration
//                if (string.IsNullOrWhiteSpace(secretKey))
//                {
//                    throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json. Please add 'Jwt:SecretKey' configuration.");
//                }

//                if (string.IsNullOrWhiteSpace(issuer))
//                {
//                    throw new InvalidOperationException("JWT Issuer is not configured in appsettings.json. Please add 'Jwt:Issuer' configuration.");
//                }

//                if (string.IsNullOrWhiteSpace(audience))
//                {
//                    throw new InvalidOperationException("JWT Audience is not configured in appsettings.json. Please add 'Jwt:Audience' configuration.");
//                }

//                // Validate SecretKey length (should be at least 32 bytes for HS256)
//                if (Encoding.UTF8.GetByteCount(secretKey) < 32)
//                {
//                    throw new InvalidOperationException("JWT SecretKey must be at least 32 bytes (256 bits) for security. Current key is too short.");
//                }

//                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//                {
//                    ValidateIssuer = true,
//                    ValidIssuer = issuer,
//                    ValidateAudience = true,
//                    ValidAudience = audience,
//                    ValidateLifetime = true,
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
//                    ClockSkew = TimeSpan.Zero
//                };
//            });

//            // Swagger/OpenAPI Configuration
//            services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EyeHospitalPOS API", Version = "v1" });

//                var securitySchema = new OpenApiSecurityScheme
//                {
//                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
//                    Name = "Authorization",
//                    In = ParameterLocation.Header,
//                    Type = SecuritySchemeType.Http,
//                    Scheme = "bearer",
//                    Reference = new OpenApiReference
//                    {
//                        Type = ReferenceType.SecurityScheme,
//                        Id = "Bearer"
//                    }
//                };

//                c.AddSecurityDefinition("Bearer", securitySchema);

//                var securityRequirement = new OpenApiSecurityRequirement
//                {
//                    { securitySchema, new[] { "Bearer" } }
//                };

//                c.AddSecurityRequirement(securityRequirement);
//            });
//        }


//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//        {
//            // ============================================
//            // MIDDLEWARE PIPELINE - ORDER IS CRITICAL
//            // ============================================

//            // 1. Exception Handling (MUST BE FIRST)
//            // Catches all exceptions in the pipeline
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//                app.UseMigrationsEndPoint();
//            }
//            else
//            {
//                app.UseExceptionHandler("/Error");
//                app.UseHsts(); // HTTP Strict Transport Security
//            }

//            // 2. HTTPS Redirection
//            // Redirects HTTP requests to HTTPS
//            app.UseHttpsRedirection();

//            // 3. Static Files
//            // Serves static files (CSS, JS, images) from wwwroot
//            app.UseStaticFiles();

//            // 4. Routing
//            // Determines which endpoint handles the request
//            app.UseRouting();

//            // 5. Session (AFTER Routing for Blazor Server)
//            // For Blazor Server, session should be after routing
//            app.UseSession();

//            // 6. Custom Middleware (after routing, before auth)
//            // Logs all requests with correlation IDs
//            // Placed after routing to have access to route information
//            app.UseMiddleware<Middleware.HttpContextLoggingMiddleware>();

//            // 7. Authentication
//            // Validates JWT tokens and establishes user identity
//            app.UseAuthentication();

//            // 8. Authorization
//            // Checks if authenticated user has permission to access resources
//            app.UseAuthorization();

//            // 9. Swagger (Development only, after auth to protect API docs)
//            if (env.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI(c => 
//                {
//                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EyeHospitalPOS API v1");
//                    c.RoutePrefix = "swagger"; // Access at /swagger
//                });
//            }

//            // 10. Endpoints (MUST BE LAST)
//            // Maps routes to controllers and Blazor components
//            app.UseEndpoints(endpoints =>
//            {
//                // API Controllers
//                endpoints.MapControllers();

//                // Blazor Server SignalR Hub
//                endpoints.MapBlazorHub();

//                // Fallback to Blazor app for all other routes
//                endpoints.MapFallbackToPage("/_Host");
//            });
//        }
//    }
//}
//*/