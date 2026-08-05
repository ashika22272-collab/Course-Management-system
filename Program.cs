using System.Reflection;
using System.Text;
using Course_Management.Interface;
using Course_Management.Repository;
using Course_Management.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ===================================
// Add Controllers
// ===================================
builder.Services.AddControllers();

// ===================================
// JWT Authentication
// ===================================
builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
				Encoding.UTF8.GetBytes(
					builder.Configuration["Jwt:Key"]!
				)
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
builder.Services.AddScoped<IAssignmentFileRepository, AssignmentFileRepository>();

// ===================================
// Swagger Configuration
// ===================================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
	// Controller Order
	options.OrderActionsBy(api =>
	{
		var controller = api.ActionDescriptor.RouteValues["controller"];

		return controller switch
		{
			"Auth" => "1",
			"Users" => "2",
			"Course" => "3",
			"Assignment" => "4",
			_ => "5"
		};
	});

	// JWT Security Definition
	options.AddSecurityDefinition(
		"Bearer",
		new OpenApiSecurityScheme
		{
			Name = "Authorization",
			Type = SecuritySchemeType.Http,
			Scheme = "Bearer",
			BearerFormat = "JWT",
			In = ParameterLocation.Header,
			Description = "Enter JWT token as: Bearer {your token}"
		});

	// JWT Security Requirement
	options.AddSecurityRequirement(document =>
		new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecuritySchemeReference(
					"Bearer",
					document
				),
				new List<string>()
			}
		});

	// XML Documentation
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

	if (File.Exists(xmlPath))
	{
		options.IncludeXmlComments(xmlPath);
	}
});

// ===================================
// Build Application
// ===================================
var app = builder.Build();

// ===================================
// Middleware Pipeline
// ===================================
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ===================================
// Enable Static Files
// ===================================
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(
		Path.Combine(builder.Environment.ContentRootPath, "Uploads")
	),
	RequestPath = "/Uploads"
});

// ===================================
// Authentication & Authorization
// ===================================
app.UseAuthentication();
app.UseAuthorization();

// ===================================
// Map Controllers
// ===================================
app.MapControllers();

app.Run();