using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlideShow.Models.ViewModel
{
    public class DoQuizModel
    {
        public List<Hint> quizHints { get; set; }
        public QuizData quizData { get; set; }
    }
}