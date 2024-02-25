using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyServiceApi.Models
{
    public class Meal
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public CourseType Course { get; set; }
    }
}