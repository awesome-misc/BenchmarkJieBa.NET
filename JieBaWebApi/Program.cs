using JiebaNet.Segmenter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddSingleton(new JiebaSegmenter());


var jiebaSegmenter = new JiebaSegmenter();
jiebaSegmenter.LoadUserDict("userdict.txt");
//jiebaSegmenter.CutInParallel()
services.AddSingleton(new JiebaSegmenter());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
