using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ekartes.Models
{
    public enum Eidos
    {
        create_d = 1,
        edit_d = 2,
        create_m = 3,
        ananeosi_m = 4,
        epanekdosi_m = 5
    }

    public enum Katastasi
    {
        anyparkto = 1,
        ekremmei = 2,
        apodekto = 3,
        oxi_apodekto = 4
    }
    public class Aitima
    {
        [Key]
        public int ID { get; set; }

        public Eidos Eidos { get; set; }

        public Katastasi Katastasi { get; set; }



        public virtual Dikaiouxos Dikaiouxos { get; set; }
        public virtual Melos Melos { get; set; }
    }
}