using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using ConsoleApp.Domain.Entities;
using ConsoleApp.Persistence.EF.Context;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.Tests;

[SimpleJob(
    RunStrategy.ColdStart,
    RuntimeMoniker.Net60,
    targetCount: 100,
    id: "Select Join Test")]
[MemoryDiagnoser]
[MinColumn]
[MaxColumn]
[MeanColumn]
[MedianColumn]
public class SelectJoinTest
{
    private const int NumberOfStudents = 999;
    private readonly List<int> _numbers = Enumerable.Range(1, NumberOfStudents).ToList();
    private readonly Random _random = new();
    private List<int> _randomNumbers;
    private int _counter;
    private List<int> numbers = Enumerable.Range(1, 100).ToList();
    private string firstName;
    private int rowCount;
    private SqlConnection connection;
    private ApplicationDbContext context;
    private int _randomCounter;

    private int GetRandomId()
    {
        return new Random().Next(1, rowCount);
    }

    private int PickNextStudentId()
    {
        var x = _counter;
        Console.WriteLine("-+-+-+-+-+  _counter:  "+_counter +"   ");
        _counter++;
        return _randomNumbers[x];
    }

    [GlobalSetup]
    public async Task Init()
    {
        _randomNumbers = _numbers.OrderBy(n => _random.Next()).ToList();

        Program.InitDapper();
        var dbContextOptions = Program.InitEf();

        context = new ApplicationDbContext(dbContextOptions);
        connection = new SqlConnection(Constants.ConnectionStringDapper);

        rowCount = await context.Students.CountAsync();
        firstName = await context.Students.OrderBy(i => Guid.NewGuid()).Select(i => i.FirstName).FirstAsync();
    }



    #region Get All

    [Benchmark(Description = "EF_Get_All_LINQ")]
    public async Task EF_Get_All_LINQ()
    {
        await context.Students.Include(s => s.Courses).ToListAsync();
    }

    [Benchmark(Description = "DP Get All Raw Sql")]
    public async Task DP_Get_All_Raw_Sql()
    {
        var identityMap = new Dictionary<int, Student>();
        (await connection
            .QueryAsync<Student, Course, Student>(@"SELECT
                        s.*,
                        c.*
                    FROM
                        student s
                    JOIN course_student cs ON
                        cs.StudentsId = s.id
                    JOIN course c ON
                        c.id = cs.CoursesId",
                (student, course) =>
                {
                    Debug.Assert(student.Id != null, "student.Id != null");
                    if (!identityMap.TryGetValue((int) student.Id, out var master))
                    {
                        identityMap[(int) student.Id] = master = student;
                    }
                    var list = master.Courses;
                    if (list == null) {
                        master.Courses = list = new List<Course>();
                    }
                    list.Add(course);
                    return master;
                }, splitOn: "id")).ToList();
    }

    #endregion

    #region Get By Id

    [Benchmark(Description = "EF Get By Id")]
    public async Task EF_Get_By_Id()
    {
       await context.Students
            .Include(s => s.Courses)
            .Where(s => s.Id == GetRandomId()).FirstOrDefaultAsync();
    }

    [Benchmark(Description = "DP Get By Id Raw Sql")]
    public async Task DP_Get_By_Id_Raw_Sql()
    {
        var identityMap = new Dictionary<int, Student>();
        (await connection
            .QueryAsync<Student, Course, Student>(@"SELECT
                        s.*,
                        c.*
                    FROM
                        student s
                    JOIN course_student cs ON
                        cs.StudentsId = s.id
                    JOIN course c ON
                        c.id = cs.CoursesId
                    WHERE s.id = @p1",
                (student, course) =>
                {
                    Debug.Assert(student.Id != null, "student.Id != null");
                    if (!identityMap.TryGetValue((int) student.Id, out var master))
                    {
                        identityMap[(int) student.Id] = master = student;
                    }
                    var list = master.Courses;
                    if (list == null) {
                        master.Courses = list = new List<Course>();
                    }
                    list.Add(course);
                    return master;
                }, new {p1 = GetRandomId()},splitOn: "id")).AsList().Take(1);
    }

    #endregion








}