using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLearning.Classes
{
    public class CourseProgress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CourseName { get; set; }
        public int Progress { get; set; }
        public string Status { get; set; }
    }
}
