using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ekartes.Models
{
    public enum FileType
    {
        Pistopoiitiko = 1,
        FATayt_d = 2,
        FATayt_m = 3,
        Photo_m = 4,
        Dikaiologitiko_Epanekdosis = 5
    }

    public class FileDikaiouxos
    {
        public int ID { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        public byte[] Content { get; set; }


        public FileType FileType { get; set; }


        public virtual Dikaiouxos Dikaiouxos { get; set; }
    }

    public class FileMelos
    {
        public int ID { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public FileType FileType { get; set; }



        public virtual Melos Melos { get; set; }
    }
}