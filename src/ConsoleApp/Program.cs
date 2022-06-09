using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using ConsoleApp.Domain.Entities;
using ConsoleApp.Persistence.Dapper.Mapping;
using ConsoleApp.Persistence.EF.Context;
using ConsoleApp.Tests;
using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Perfolizer.Mathematics.Distributions;

namespace ConsoleApp;

public static class Program
{
    private static async Task Main(string[] args)
    {
        // var dbContextOptions = InitEf();
        // var context = new ApplicationDbContext(dbContextOptions);
        // await context.Database.EnsureCreatedAsync();
        // InitDapper();
        // var connection = new SqlConnection(Constants.ConnectionStringDapper);

        // int x = 1;
        // NOTE: Initialized

        // var joinDataInitializer = new JoinDataInitializer(context);
        // await joinDataInitializer.InitData();

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


        // Console.ReadLine();
        // Environment.Exit(0);
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