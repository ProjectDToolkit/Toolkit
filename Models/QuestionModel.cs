using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectD.Models
{
    public class QuestionModel
    {
        public int questionId { get; set; }
        public string SessionId { get; set; }
        public string question { get; set; }

        public bool isAnswered { get; set; }
    }
}
