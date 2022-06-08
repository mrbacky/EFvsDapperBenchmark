using System;
using BenchmarkDotNet.Running;
using ConsoleApp.Persistence.Dapper.Mapping;
using ConsoleApp.Persistence.EF.Context;
using ConsoleApp.Tests;
using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp;

public static class Program
{
    private static void Main(string[] args)
    {
        var dbContextOptions = InitEf();
        var context = new ApplicationDbContext(dbContextOptions);
        context.Database.EnsureCreatedAsync();
        Console.WriteLine("In Main");
        var joinDataInitializer = new JoinDataInitializer(context);
        joinDataInitializer.Init();

        // context.Database.Migrate();

        // InitDapper();

        // BenchmarkRunner.Run<InsertTest>();
        // BenchmarkRunner.Run<InsertManyTest>();
        BenchmarkRunner.Run<SelectJoinTest>();
        // BenchmarkRunner.Run<SelectTest>();
        // BenchmarkRunner.Run<SearchTest>();
        // BenchmarkRunner.Run<FunctionsTest>();
        // BenchmarkRunner.Run<UpdateTest>();
        // BenchmarkRunner.Run<DeleteTest>();


        Console.ReadLine();
    }

    public static void InitDapper()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        FluentMapper.Initialize(config =>
        {
            config.AddMap(new StudentMap());
            config.ForDommel();
        });
    }

    public static DbContextOptions InitEf()
    {
        var builder = new DbContextOptionsBuilder();
        builder.UseSqlServer(Constants.ConnectionStringEF);
        builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        return builder.Options;
    }
}