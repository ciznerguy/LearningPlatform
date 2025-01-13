using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningClassLibary.Models
{
    public class ImageModel
    {
        public int ImageID { get; set; }
        public int OwnerID { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; }
        public DateTime UploadDate { get; set; }
    }

}
