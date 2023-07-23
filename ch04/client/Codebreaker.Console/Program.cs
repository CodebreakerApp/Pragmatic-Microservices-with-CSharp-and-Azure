﻿using Codebreaker.Client;
using Codebreaker.GameAPIs.Client;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient<GamesClient>(client =>
{
    string gamesUrl = builder.Configuration["GamesApiUrl"] ?? throw new InvalidOperationException("GamesApiUrl not found");
    client.BaseAddress = new Uri(gamesUrl);
});

builder.Services.AddTransient<Runner>();
var app = builder.Build();

var runner = app.Services.GetRequiredService<Runner>();
await runner.StartAsync();
