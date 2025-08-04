using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ekartes.Models
{
    public enum Rolos
    {
        Admin = 1,
        User = 2
    }
    public enum KatastasiD : int
    {
        ΕΝ_ΕΝΕΡΓΕΙΑ = 10,
        ΕΝ_ΑΠΟΣΤΡΑΤΕΙΑ = 20
    }
    public enum Vathmos
    {
        ΔΝΕΑΣ, ΛΧΙΑΣ, ΕΠΧΙΑΣ, ΑΛΧΙΑΣ, ΑΝΘΣΤΗΣ, ΑΝΘΛΓΟΣ, ΥΠΛΓΟΣ, ΛΓΟΣ, ΤΧΗΣ, ΑΝΧΗΣ, ΣΧΗΣ, ΤΑΞΧΟΣ, ΥΠΤΓΟΣ, ΑΝΤΓΟΣ
    }

    public class Dikaiouxos
    {
        [Key]
        public int ID { get; set; }

        [DisplayName("ΑΜ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public int AM { get; set; }

        [DisplayName("Όνομα")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public string Onoma { get; set; }

        [DisplayName("Επίθετο")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public string Epitheto { get; set; }

        [DisplayName("ΑΤ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public string AT { get; set; }

        [DisplayName("Μονάδα")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public string Monada { get; set; }

        [DisplayName("Βαθμός")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public Vathmos? Vathmos { get; set; }

        [DisplayName("Ο-Σ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public string O_S { get; set; }

        [DisplayName("Κατάσταση")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public KatastasiD KatastasiD { get; set; }

        [DisplayName("Ρόλος")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        public Rolos Rolos { get; set; }

        [DisplayName("Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Συνθηματικό")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Το πεδίο  είναι υποχρεωτικό.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        [DisplayName("Επιβεβαίωση Συνθηματικού")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Το Συνθηματικό και το Συνθηματικό Επιβεβαίωσης δεν ταιριάζουν.")]
        public string ConfirmPassword { get; set; }




        public virtual ICollection<Melos> Meli { get; set; }
        public virtual ICollection<FileDikaiouxos> FilesDikaiouxos { get; set; }
        public virtual ICollection<Aitima> Aitimata { get; set; }

    }
}