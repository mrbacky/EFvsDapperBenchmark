using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsoleApp.Domain.Entities;

public class Course
{
    public int? Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public List<Student> Students { get; set; }
}