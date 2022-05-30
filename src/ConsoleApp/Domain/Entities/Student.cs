using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApp.Domain.Entities
{
    public class Student
    {
        public int? Id { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public DateTime BirthDate { get; set; }
        public List<Course> Courses { get; set; }
    }
}
