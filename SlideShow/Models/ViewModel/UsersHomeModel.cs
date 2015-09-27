using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlideShow.Models
{
    public class UsersHomeModel
    {

        public int MadeQuizNum { get; set; }

        public int AnsweredQuizNum { get; set; }

        public List<QUIZ> RecentMadeQuiz { get; set; }

        public List<QUIZ> RecentAnsweredQuiz { get; set; }

    }

}