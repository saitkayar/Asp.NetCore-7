using Microsoft.AspNetCore.OutputCaching;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddOutputCache();

#region policy or caching

builder.Services.AddOutputCache(opt =>
{
    //default deðiþti
    opt.AddBasePolicy(bl =>
    {
        bl.Expire(TimeSpan.FromSeconds(5));
    });

    //cusstom policy
    opt.AddPolicy("custom", p =>
    {
        p.Expire(TimeSpan.FromSeconds(5));
    };
});
#endregion


var app = builder.Build();

app.UseOutputCache();

//cache output
app.MapGet("/", () =>
{
    return Results.Ok(DateTime.Now);
}).CacheOutput();

//outputcache
app.MapGet("merhaba", [OutputCache()]() =>
{
    return Results.Ok(DateTime.Now);
}).CacheOutput();

//controllerdee cache:controller üzerine [outputcache] yaz]





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
