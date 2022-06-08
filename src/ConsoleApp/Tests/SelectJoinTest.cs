using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using ConsoleApp.Persistence.EF.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Tests;

[SimpleJob(
    RunStrategy.ColdStart,
    RuntimeMoniker.Net60,
    1,
    targetCount: 5,
    id: "Select Test")]
[MemoryDiagnoser]
[MinColumn]
[MaxColumn]
[MeanColumn]
[MedianColumn]
public class SelectJoinTest
{
    private SqlConnection connection;
    private ApplicationDbContext context;

    // private int GetRandomId() => new Random().;
    private int counter;
    private string firstName;

    private List<int> numbers = Enumerable.Range(1, 100).ToList();

    private List<int> randomNumbers = new();
    private int rowCount;

    public List<int> GenerateRandomNumbers(int from, int to, int numberOfElement)
    {
        var random = new Random();
        var numbers = new HashSet<int>();
        while (numbers.Count < numberOfElement) numbers.Add(random.Next(from, to));

        return numbers.ToList();
    }

    // private int PickNumber()
    // {
    //     int randomNumber = GetRandomId();
    //     if (numbers.Contains())
    //     {
    //
    //     }
    // }

    private int GetRandomId()
    {
        var x = counter;
        counter++;
        // Console.WriteLine("In GetRandomId > x: " + x);
        // Console.WriteLine("In GetRandomId > list:  " + randomNumbers);
        return randomNumbers[x];
    }

    [GlobalSetup]
    public async Task Init()
    {
        randomNumbers = GenerateRandomNumbers(1, 100, 10);
        Program.InitDapper();
        var dbContextOptions = Program.InitEf();

        context = new ApplicationDbContext(dbContextOptions);
        connection = new SqlConnection(Constants.ConnectionStringDapper);

        rowCount = await context.Students.CountAsync();
        firstName = await context.Students.OrderBy(i => Guid.NewGuid()).Select(i => i.FirstName).FirstAsync();
    }

    #region Get Student By Id with Courses

    [Benchmark(Description = "DP Select Student By Id with Courses RawSql")]
    public async Task DP_Select_Student_By_Id_With_Courses_RawSqwl()
    {
        // Console.WriteLine("*** Test");
        // foreach (var n in randomNumbers)
        // {
        var rand = GetRandomId();
        Console.WriteLine("Random id from METHOD: " + rand);

        // }
        // int id = GetRandomId();
        //
        //
        //
        // Console.WriteLine("----------------------------------");
        // Console.WriteLine("**********  Generated id: " + id);
        // Console.WriteLine("----------------------------------");
        await context
            .Students
            .FromSqlRaw(@"SELECT
	                                    s.*,
                                        c.*
                                    FROM
	                                    student s
                                    JOIN course_student cs ON
	                                    cs.StudentsId = s.id
                                    JOIN course c ON
	                                    c.id = cs.CoursesId
                                    WHERE
	                                    s.id = {0} ", rand).SingleOrDefaultAsync();
    }

    #endregion

    #region Find Single

    // [Benchmark(Description = "EF Find")]
    // public async Task EF_Select_Student_By_Id_Linq()
    // {
    //     int id = GetRandomId();
    //     await context.Students.FindAsync(id);
    // }
    //
    // [Benchmark(Description = "DP Find")]
    // public async Task DP_Select_Student_By_Id_Linq()
    // {
    //     int id = GetRandomId();
    //     await connection.GetAsync<Student>(id);
    // }

    #endregion

    #region SingleOrDefault RawSql

    // [Benchmark(Description = "EF SingleOrDefault RawSql")]
    // public async Task EF_Select_Student_By_Id_RawSqwl()
    // {
    //     int id = GetRandomId();
    //     await context.Students.FromSqlRaw("SELECT * from student WHERE id = {0}", id).SingleOrDefaultAsync();
    // }
    //
    // [Benchmark(Description = "DP SingleOrDefault RawSql")]
    // public async Task DP_Select_Student_By_Id()
    // {
    //     int id = GetRandomId();
    //     await connection.QuerySingleOrDefaultAsync<Student>("SELECT * from student WHERE id = @pid", new { pid = id });
    // }

    #endregion

    #region Filtered By FirstName

    // [Benchmark(Description = "EF Filter By FirstName LinQ")]
    // public async Task EF_FilterBy_FirstName_LinQ()
    // {
    //     await context.Students.Where(i => i.FirstName == firstName).ToListAsync();
    // }
    //
    //
    // [Benchmark(Description = "DP Filter By FirstName LinQ")]
    // public async Task DP_FilterBy_FirstName_LinQ()
    // {
    //     (await connection.SelectAsync<Student>(i => i.FirstName == firstName)).ToList();
    // }
    //
    //
    // [Benchmark(Description = "EF Filter By FirstName RawSql")]
    // public async Task EF_FilterBy_FirstName_RawSql()
    // {
    //     await context.Students.FromSqlRaw("SELECT * from student WHERE first_name = {0}", firstName).ToListAsync();
    // }
    //
    // [Benchmark(Description = "DP Filter By FirstName RawSql")]
    // public async Task DP_FilterBy_FirstName_RawSql()
    // {
    //     (await connection.QueryAsync<Student>("SELECT * from student WHERE first_name = @FirstName", new { FirstName = firstName })).ToList();
    // }

    #endregion

    #region Get All

    //
    // [Benchmark(Description = "EF Get ALL")]
    // public async Task EF_Select_Student_ALL()
    // {
    //     await context.Students.ToListAsync();
    // }
    //
    //
    // [Benchmark(Description = "DP Get ALL")]
    // public async Task DP_Select_Student_ALL()
    // {
    //     (await connection.GetAllAsync<Student>()).ToList();
    // }

    #endregion
}