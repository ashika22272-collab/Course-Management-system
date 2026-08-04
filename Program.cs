using System.Reflection;
using System.Text;
using Course_Management.Interface;
using Course_Management.Repository;
using Course_Management.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ===================================
// Controllers
// ===================================
builder.Services.AddControllers();


// ===================================
// JWT Authentication
// ===================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
			),

			ClockSkew = TimeSpan.Zero
		};
	});


builder.Services.AddAuthorization();


// ===================================
// Dependency Injection
// ===================================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();


// ===================================
// Swagger
// ===================================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("Bearer",
		new OpenApiSecurityScheme
		{
			Name = "Authorization",
			Type = SecuritySchemeType.Http,
			Scheme = "Bearer",
			BearerFormat = "JWT",
			In = ParameterLocation.Header,
			Description = "Enter JWT token as: Bearer {your token}"
		});


	options.AddSecurityRequirement(document =>
		new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecuritySchemeReference(
					"Bearer",
					document),
				new List<string>()
			}
		});
});


// ===================================
// Build
// ===================================
var app = builder.Build();


// ===================================
// Pipeline
// ===================================
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