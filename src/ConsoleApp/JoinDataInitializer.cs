using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using ConsoleApp.Domain.Entities;
using ConsoleApp.Persistence.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp;

public class JoinDataInitializer
{
    // Setup
    private const int NumberOfCourses = 5000;
    private const int NumberOfStudents = 1000;
    private readonly ApplicationDbContext _context;
    private readonly List<int> _numbers = Enumerable.Range(1, NumberOfCourses).ToList();
    private readonly Random _random = new();
    private readonly List<int> _randomNumbers;
    private int _counter;

    public JoinDataInitializer(ApplicationDbContext context)
    {
        _context = context;
        _randomNumbers = _numbers.OrderBy(n => _random.Next()).ToList();
    }

    private int PickNextCourseId()
    {
        var x = _counter;
        Console.WriteLine("x counter : " + x);
        _counter++;
        return _randomNumbers[x];
    }

    public async Task InitData()
    {
        await InitStudents();
        await InitCourses();
        await InitCourseStudent();
    }

    private async Task InitStudents()
    {
        var studentFaker = new Faker<Student>("tr")
            .RuleFor(i => i.FirstName, i => i.Person.FirstName)
            .RuleFor(i => i.LastName, i => i.Person.LastName)
            .RuleFor(i => i.BirthDate, i => i.Person.DateOfBirth);
        var students = studentFaker.Generate(NumberOfStudents);
        Console.WriteLine("Student count: " + students.Count);

        await _context.AddRangeAsync(students);
        await _context.SaveChangesAsync();
    }

    private async Task InitCourses()
    {
        var courseFaker = new Faker<Course>("tr")
            .RuleFor(i => i.Name, i => i.Company.CompanyName());
        var courses = courseFaker.Generate(NumberOfCourses);
        Console.WriteLine("Course count: " + courses.Count);
        await _context.Courses.AddRangeAsync(courses);
        await _context.SaveChangesAsync();
    }

    private async Task InitCourseStudent()
    {
        const string sql = @"INSERT INTO course_student (CoursesId, StudentsId)VALUES ({0}, {1})";
        for (var studentId = 1; studentId <= NumberOfStudents; studentId++)
        {
            for (var i = 1; i <= 5; i++)
            {
                Console.WriteLine("i: " + i + " student id: " + studentId);
                var courseId = PickNextCourseId();
                Console.WriteLine("courseId: " + courseId + " -  studentId: " + studentId);
                await _context.Database.ExecuteSqlRawAsync(sql, courseId, studentId);
            }
        }

        // return Task.CompletedTask;
        await _context.SaveChangesAsync();
    }
}