var builder = WebApplication.CreateBuilder (args);

builder.ConfigureIoCContainer ();

var app = builder.Build ();

app.ConfigureMiddleware (builder.Configuration);

app.Run ();