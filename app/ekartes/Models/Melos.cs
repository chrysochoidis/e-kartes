using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ekartes.Models
{
    public enum Sygeneia
    {
        ΠΑΤΕΡΑΣ, ΜΗΤΕΡΑ, ΥΙΟΣ, ΚΟΡΗ, ΣΥΖΗΓΟΣ, ΠΕΘΕΡΟΣ, ΠΕΘΕΡΑ
    }
    public enum Aitiologia
    {
        ΑΡΧΙΚΗ = 1,
        ΚΛΟΠΗ = 2,
        ΦΘΟΡΑ = 3,
        ΑΠΩΛΕΙΑ = 4
    }

    public class Melos
    {
        [Key]
        public int ID { get; set; }

        [DisplayName("ΑΤ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public string AT { get; set; }

        [DisplayName("Όνομα")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public string Onoma { get; set; }

        [DisplayName("Επίθετο")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public string Epitheto { get; set; }

        [DisplayName("Συγγένεια")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public Sygeneia? Sygeneia { get; set; }

        [DisplayName("Ημερομηνία έκδοσης")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime HmniaEkdosis { get; set; }

        [DisplayName("Ημερομηνία λήξης")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime HmniaLiksis { get; set; }

        [DisplayName("Κωδικός Κάρτας")]
        public string KwdikosKartas { get; set; }


        [DisplayName("Αιτιολογία Έκδοσης")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public Aitiologia? Aitiologia { get; set; }



        public virtual Dikaiouxos Dikaiouxos { get; set; }
        public virtual ICollection<FileMelos> FilesMelos { get; set; }
        public virtual ICollection<Aitima> Aitimata { get; set; }
    }
}