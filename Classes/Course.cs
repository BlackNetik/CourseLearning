using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLearning.Classes
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public User User { get; set; }
    }
}
