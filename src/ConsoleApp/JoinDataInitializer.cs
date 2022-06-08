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
    private readonly ApplicationDbContext _context;
    private readonly List<int> _randomNumbers;

    // private int GetRandomId() => new Random().Next(1, 10);
    private int counter;

    public JoinDataInitializer(ApplicationDbContext context)
    {
        _context = context;
        _randomNumbers = GenerateRandomNumbers(1, 10000, 10000);
    }

    private int GetRandomId()
    {
        var x = counter;
        counter++;
        // Console.WriteLine("In GetRandomId > x: " + x);
        // Console.WriteLine("In GetRandomId > list:  " + randomNumbers);
        return _randomNumbers[x];
    }

    public List<int> GenerateRandomNumbers(int from, int to, int numberOfElement)
    {
        var random = new Random();
        var numbers = new HashSet<int>();
        while (numbers.Count <= numberOfElement) numbers.Add(random.Next(from, to));
        return numbers.ToList();
    }

    public async Task Init()
    {
        Console.WriteLine("IN  INIT");

        await InitStudents();
        await InitCourses();
        await InitCourseStudent();
    }

    public async Task InitStudents()
    {
        Console.WriteLine("IN Student INIT");
        var studentFaker = new Faker<Student>("tr")
            .RuleFor(i => i.FirstName, i => i.Person.FirstName)
            .RuleFor(i => i.LastName, i => i.Person.LastName)
            .RuleFor(i => i.BirthDate, i => i.Person.DateOfBirth);
        var students = studentFaker.Generate(10000);

        await _context.AddRangeAsync(students);
        await _context.SaveChangesAsync();
    }

    public async Task InitCourses()
    {
        var courseFaker = new Faker<Course>("tr")
            .RuleFor(i => i.Name, i => i.Commerce.Department());
        var courses = courseFaker.Generate(2000);
        await _context.Courses.AddRangeAsync(courses);
        await _context.SaveChangesAsync();
    }

    public async Task InitCourseStudent()
    {
        Console.WriteLine("LIST: " + _randomNumbers.Count);

        var sql = @"INSERT INTO course_student (CoursesId, StudentsId)
                                VALUES ({0}, {1})";
        for (var studentId = 1; studentId <= 1000; studentId++)
        {
            var courseId = GetRandomId();
            Console.WriteLine("courseId: " + courseId + " *  studentId: " + studentId);
            await _context.Database.ExecuteSqlRawAsync(sql, GetRandomId(), studentId);
        }

        await _context.SaveChangesAsync();
    }
}