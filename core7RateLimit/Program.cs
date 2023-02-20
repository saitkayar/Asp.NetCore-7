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
//sabit zaman aral���nda �al���r
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

#region Sliding W�ndow
//
builder.Services.AddRateLimiter(opt =>
{//fixed benzer sabit s�rede zaman aral��u�nda istekleri yapar lakin  s�renn yar�s�nda di�er periyodun rrequestinden kotas�ndan istekleri kar��lar.
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
//her periyotta request say�s� kadar token �retilir.E�er tokenlar kullan�ld�ysa di�er periyottan bor� al�na bilir.maxiumum token verdi�imiz sabit token kadard�r.
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
//asenkron requestleri s�n�rlamak i�in kullan�l�r.her iistek concurrenncy s�n�r�n� bir azalt�r .istek tamamlan�rsa bir art�r�r s�n�r�.sadece asenkron requestlerde �al���r.
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
//controller veya action �st�ne ekle ve �al��t�r
#endregion
#region DisableRateLimiting    
//controller veya action �st�ne ekle ve disable et
#endregion

#region minimal apida ratelimiter


#endregion

#region onrejected

//s�n�rdan dolay� bo�a ��kan requestlerin  loglama vs.i�elmleri yapmak i�in kullan�r�z.event mant���nda �al���r.

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

//IRateLimiterPolicy kullanarak bir class o�utur sonra da program cs ekle

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
