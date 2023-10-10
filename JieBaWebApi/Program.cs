using JiebaNet.Segmenter;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var configuration = builder.Configuration;

var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
Directory.SetCurrentDirectory(directory!);

JiebaNet.Segmenter.ConfigManager.ConfigFileBaseDir = configuration.GetValue<string>("Jieba:ConfigFilesBaseDirectory");

services.AddSingleton(new JiebaSegmenter());

//var lazyJiebaSegmenter = new JiebaSegmenter();
//lazyJiebaSegmenter.LoadUserDict(@"Resources\user_dict.txt");
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
