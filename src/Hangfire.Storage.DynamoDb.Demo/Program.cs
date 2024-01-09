using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Hangfire.Storage.DynamoDb;
using Hangfire.Storage.DynamoDb.Demo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSService<IAmazonDynamoDB>(new AWSOptions
{
    DefaultClientConfig = { ServiceURL = "http://localhost:8000", UseHttp = true }
});


builder.Services.AddHangfire(configuration =>
{
    var sp = builder.Services.BuildServiceProvider();

    configuration.UseDynamoDbStorage(sp.GetRequiredService<IAmazonDynamoDB>());
    // configuration.UseRedisStorage("localhost:36389");
});

var app = builder.Build();

app.UseHangfireDashboard();

app.UseHangfireServer();

RecurringJob.AddOrUpdate<RecurringJobDemo>(
    "WhatTimeIsIt",
    job => job.Run(),
    Cron.Minutely()
);

app.Run();