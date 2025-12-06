using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskCollaborationAppAPI.Data;
using TaskCollaborationAppAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

/* In memory Db */
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("FinalDb-DanyKo-GabrielSiewert"));

/* Register Repository, Unit Of Work, Service patterns */
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

/* JWT and Google */

// builder.Services.AddSingleton<JwtAuthService>();

var jwtSecret = builder.Configuration["JwtSettings:Secret"];

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

///* Cookie Authentication from google.com */
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//}).AddCookie(options =>
//{
//    options.LoginPath = "/auth/login";
//    options.AccessDeniedPath = "/auth/denied";
//    options.LogoutPath = "/auth/logout";
//}).AddGoogle(options =>
//{
//    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
//    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
//    options.Scope.Add("profile");
//    options.Scope.Add("email");
//}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret))
//    };

//    options.Events = new JwtBearerEvents
//    {
//        OnMessageReceived = context =>
//        {
//            var sessionToken = context.HttpContext.Session.GetString("JwtToken");
//            if (!string.IsNullOrWhiteSpace(sessionToken))
//            {
//                context.Token = sessionToken;
//                return TaskItem.CompletedTask;
//            }

//            return TaskItem.CompletedTask;
//        }
//    };
//});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Used for in memory database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
