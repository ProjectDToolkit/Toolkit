using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectD.Models
{
    public class FileModel
    {
        public int idFiles{ get; set; }
        public string idSession{ get; set; }
        public string fileName { get; set; }
        public string fileDesc { get; set; }

        public string filePath { get; set; }
    }
}
