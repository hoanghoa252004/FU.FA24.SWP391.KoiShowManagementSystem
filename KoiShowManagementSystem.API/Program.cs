using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using KoiShowManagementSystem.Repositories.MyDbContext;
using KoiShowManagementSystem.API.Helper;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using KoiShowManagementSystem.Repositories.Helper;

namespace KoiShowManagementSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("appsettings.Secret.json", optional: true, reloadOnChange: true);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }); ; ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //*************
            builder.Services.Configure<IdentityOptions>(
                opt => opt.SignIn.RequireConfirmedEmail = true
                );
            // Đăng kí DBContext: OK
            builder.Services.AddDbContext<KoiShowManagementSystemContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("cnn"));
            });
            builder.Services.AddHttpContextAccessor();
            // Đăng kí Services Layer: OK
            builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<S3UploadService>();
            builder.Services.AddScoped<JwtServices>();
            builder.Services.AddScoped<Repository>();
            builder.Services.AddScoped<IShowService, ShowService>();
            builder.Services.AddScoped<IKoiService, KoiService>();
            builder.Services.AddScoped<IUserService,UserService>();
            builder.Services.AddScoped<IRegistrationService, RegistrationService>();
            //builder.Services.AddScoped<IShowService, ShowService>();
            builder.Services.AddScoped<IRefereeService, RefereeService>();
            // Thêm Schema & Params dùng validate Token:
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Các params cần check:
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,
                        // Valid argument:
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Thay bằng giá trị của bạn
                        ValidAudience = builder.Configuration["Jwt:Audience"],  // Thay bằng giá trị của bạn
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))  // Thay bằng khóa bí mật của bạn
                    };
                });
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.WithOrigins("http://localhost:9999") // Allow the frontend's origin
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()); // If you're using credentials (cookies, Authorization headers, etc.)
            });
            //
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors();

            app.UseHttpsRedirection();


            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
