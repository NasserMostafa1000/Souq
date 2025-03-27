using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineStoreAPIs.Hubs;
using StoreBusinessLayer.AdminInfo;
using StoreBusinessLayer.Carts;
using StoreBusinessLayer.Clients;
using StoreBusinessLayer.Orders;
using StoreBusinessLayer.Products;
using StoreBusinessLayer.Shipping;
using StoreBusinessLayer.Users;
using StoreDataAccessLayer;
using System.Text;

namespace OnlineStoreAPIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbcontext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr")));

            // تسجيل خدمات الأعمال (Business Layer)
            builder.Services.AddScoped<CategoriesBL>();
            builder.Services.AddScoped<ShippingBL>();
            builder.Services.AddScoped<UsersBL>();
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddScoped<ClientsBL>();
            builder.Services.AddScoped<ProductsBL>();
            builder.Services.AddScoped<ColorsBL>();
            builder.Services.AddScoped<SizesBL>();
            builder.Services.AddScoped<OrdersBL>();
            builder.Services.AddScoped<CartsBL>();
            builder.Services.AddScoped<AdminInfoBL>();
            builder.Services.AddSignalR();

            // إعداد المصادقة باستخدام JWT
            var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                    
                });

            // تفعيل التصريح (Authorization)
            builder.Services.AddAuthorization();

            // تفعيل CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin() // السماح بأي مصدر
                              .AllowAnyMethod() // السماح بأي طريقة (GET, POST, PUT, DELETE, ...)
                              .AllowAnyHeader(); // السماح بأي رأس (Header)
                    });
            });


          //  builder.Services.AddSignalR();

            var app = builder.Build();
            // ترتيب الـ Middleware بشكل صحيح
            app.UseCors("AllowAll"); // تأكد من وضعه قبل UseRouting()
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllers();
                _ =endpoints.MapHub<OrderHub>("/orderHub"); // لتحديد مسار الـ SignalR Hub
            });

            app.UseHttpsRedirection();

            // تفعيل Swagger في وضع التطوير
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.Run();
        }
    }
}
