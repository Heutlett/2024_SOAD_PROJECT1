using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyServiceApi.Models
{
    public class Meal
    {
        public string? Name { get; set; }
        public CourseType Course { get; set; } = CourseType.MainCourse;
    }
}