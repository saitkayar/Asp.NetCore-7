using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region Basic RateLimit

builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter("Basic", _opt =>
    {
        _opt.Window = TimeSpan.FromSeconds(12);
        _opt.PermitLimit = 4;
        _opt.QueueLimit = 2;
        _opt.QueueProcessingOrder =QueueProcessingOrder.OldestFirst;
    });
});

#endregion

#region Fixed window 
//sabit zaman aralýðýnda çalýþýr
builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter("Fixed", _opt =>
    {
        _opt.Window = TimeSpan.FromSeconds(12);
        _opt.PermitLimit = 4;
        _opt.QueueLimit = 2;
        _opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

#region Sliding WÝndow
//
builder.Services.AddRateLimiter(opt =>
{//fixed benzer sabit sürede zaman aralýðuýnda istekleri yapar lakin  sürenn yarýsýnda diðer periyodun rrequestinden kotasýndan istekleri karþýlar.
    opt.AddSlidingWindowLimiter("Sliding", _opt =>
    {
        _opt.Window = TimeSpan.FromSeconds(12);
        _opt.PermitLimit = 4;
        _opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        _opt.QueueLimit = 2;
        _opt.SegmentsPerWindow = 2;
    });
});


#endregion


#region Token Bucket
//her periyotta request sayýsý kadar token üretilir.Eðer tokenlar kullanýldýysa diðer periyottan borç alýna bilir.maxiumum token verdiðimiz sabit token kadardýr.
builder.Services.AddRateLimiter(opt =>
{
    opt.AddTokenBucketLimiter("Token", _opt =>
    {
        _opt.TokenLimit = 4;
        _opt.TokensPerPeriod = 4;
        _opt.QueueLimit = 2;
        _opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        _opt.ReplenishmentPeriod = TimeSpan.FromSeconds(12);

    });

});


#endregion

#region Concurrency
//asenkron requestleri sýnýrlamak için kullanýlýr.her iistek concurrenncy sýnýrýný bir azaltýr .istek tamamlanýrsa bir artýrýr sýnýrý.sadece asenkron requestlerde çalýþýr.
builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter("Concurrency", _opt =>
    {
     
        _opt.PermitLimit = 4;
        _opt.QueueLimit = 2;
        _opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

#endregion

#region EnableRateLimiting    
//controller veya action üstüne ekle ve çalýþtýr
#endregion
#region DisableRateLimiting    
//controller veya action üstüne ekle ve disable et
#endregion

#region minimal apida ratelimiter


#endregion

#region onrejected

//sýnýrdan dolayý boþa çýkan requestlerin  loglama vs.iþelmleri yapmak için kullanýrýz.event mantýðýnda çalýþýr.

builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter("Basic", _opt =>
    {
        _opt.Window = TimeSpan.FromSeconds(12);
        _opt.PermitLimit = 4;
        _opt.QueueLimit = 2;
        _opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    opt.OnRejected = (context, cancellationToken) => {
        
        ///log
        return new(); };
});

#endregion


#region customRatelimiter

//IRateLimiterPolicy kullanarak bir class oþutur sonra da program cs ekle

builder.Services.AddRateLimiter(opt =>
{
 // opt.AddPolicy<string,"class ismi" > ("class ismi");
});

#endregion
var app = builder.Build();
app.UseRateLimiter();
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
