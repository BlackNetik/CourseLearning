using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLearning.Classes
{
    public class PageObject
    {
        public int page_number { get; set; }
        public string header { get; set; }
        public string text { get; set; }
        public TestObject standardized_test { get; set; }
        public string question { get; set; }
        public string correct_answer { get; set; }
    }

    public class TestObject
    {
        public string question { get; set; }
        public string[] answer_options { get; set; }
        public int correct_answer { get; set; }
    }
}
