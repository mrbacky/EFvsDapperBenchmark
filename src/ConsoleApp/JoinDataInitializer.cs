using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using ConsoleApp.Domain.Entities;
using ConsoleApp.Persistence.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp;

public class JoinDataInitializer
{
    private ApplicationDbContext _context;
    private int GetRandomId() => new Random().Next(1, 10);
    public JoinDataInitializer(ApplicationDbContext context)
    {
        _context = context;
    }
    public  async Task Init()
    {
        await InitStudents();
        await InitCourses();
        await InitCourseStudent();
    }

    public async Task InitStudents()
    {
        var studentFaker = new Faker<Student>("tr")
            .RuleFor(i => i.FirstName, i => i.Person.FirstName)
            .RuleFor(i => i.LastName, i => i.Person.LastName)
            .RuleFor(i => i.BirthDate, i => i.Person.DateOfBirth);

        var students = studentFaker.Generate(100);

        await _context.AddRangeAsync(students);
        await _context.SaveChangesAsync();
    }

    public async Task InitCourses()
    {


        var courseFaker = new Faker<Course>("tr")
        .RuleFor(i => i.Name, i => i.Commerce.Department());
        var courses = courseFaker.Generate(10);
        var students = _context.Students.ToListAsync();

        await _context.Courses.AddRangeAsync(courses);
        await _context.SaveChangesAsync();
    }

    public async Task InitCourseStudent()
    {
        string sql = @"INSERT INTO course_student (CoursesId, StudentsId)
                                VALUES ({0}, {1})";
        for (int studentId = 1; studentId <= 20; studentId++)
        {

            for (int courseId = 1; courseId <= 5 ; courseId++)
            {
                Console.WriteLine("courseId: " + courseId + " *  studentId: " + studentId);
                await _context.Database.ExecuteSqlRawAsync(sql, courseId, studentId);
            }
        }
        await _context.SaveChangesAsync();
    }
}