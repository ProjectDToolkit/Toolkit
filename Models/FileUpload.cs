using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;


namespace ProjectD.Models
{
    public class FileUpload
    {
        public string IdSession { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }

        [DisplayName("File Desc")]
        public string FileDesc { get; set; }

        [DisplayName("File Path")]
        public string FilePath { get; set; }

        public HttpPostedFileBase UploadFile { get; set; }
    }
}
