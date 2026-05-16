using ClassManagementAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddScoped<ClassManagementAPI.Repositories.IUserRepository, ClassManagementAPI.Repositories.UserRepository>();
builder.Services.AddScoped<ClassManagementAPI.Services.IUserService, ClassManagementAPI.Services.UserService>();
builder.Services.AddScoped<ClassManagementAPI.Repositories.IClassRepository, ClassManagementAPI.Repositories.ClassRepository>();
builder.Services.AddScoped<ClassManagementAPI.Services.IClassService, ClassManagementAPI.Services.ClassService>();
builder.Services.AddScoped<ClassManagementAPI.Repositories.IAttendanceRepository, ClassManagementAPI.Repositories.AttendanceRepository>();
builder.Services.AddScoped<ClassManagementAPI.Services.IAttendanceService, ClassManagementAPI.Services.AttendanceService>();
builder.Services.AddScoped<ClassManagementAPI.Repositories.IScheduleRepository, ClassManagementAPI.Repositories.ScheduleRepository>();
builder.Services.AddScoped<ClassManagementAPI.Services.IScheduleService, ClassManagementAPI.Services.ScheduleService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!context.Users.Any(u => u.Email == "nguyenhoang300281@gmail.com"))
    {
        context.Users.Add(new ClassManagementAPI.Models.User
        {
            Email = "nguyenhoang300281@gmail.com",
            FullName = "Administrator",
            Role = "Admin",
            Password = BCrypt.Net.BCrypt.HashPassword("123456")
        });
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
