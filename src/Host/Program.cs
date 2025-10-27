using Data;
using Host;

var builder = WebApplication.CreateBuilder(args);


builder.ConfigureServices();

var app = builder.Build();

app.ConfigureApp();

app.Run();
