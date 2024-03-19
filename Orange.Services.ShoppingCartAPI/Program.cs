using Microsoft.EntityFrameworkCore;
using Orange.Services.ShoppingCartAPI.Data;
using Orange.Services.ShoppingCartAPI.Extensions;
using Orange.Services.ShoppingCartAPI.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrangeDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.AddAppAuthentication();

builder.Services.AddAuthorization();

var app = builder.Build();

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

ApplyMigrations();

app.Run();


void ApplyMigrations()
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<OrangeDbContext>();

        if (db.Database.GetPendingMigrations().Count() > 0)
        {
            db.Database.Migrate();
        }
    }
}
