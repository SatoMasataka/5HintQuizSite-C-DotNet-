using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SlideShow.Models.ViewModel
{
    public class MadeQuizListModel
    {
        public IEnumerable<QuizData> MadeQuizList { get; set; }
    }

    public class QuizData
    {
        public QUIZ Quiz { get; set; }
        public string CategoryName { get; set; }
        public string UserName { get; set; }
    }
}