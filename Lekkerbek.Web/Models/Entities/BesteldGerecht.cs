using System.ComponentModel.DataAnnotations;

namespace Lekkerbek.Web.Models.Entities
{
    public class BesteldGerecht
    {
        public int Id { get; set; }

        [Display(Name = "Bestelling")]
        public int BestellingId { get; set; }
        public Bestelling Bestelling { get; set; }

        [Display(Name = "Gerecht")]
        public int GerechtId { get; set; }
        public Gerecht Gerecht { get; set; }

        [Range(1, int.MaxValue)]
        public int Aantal { get; set; }

        public string Opmerkingen { get; set; }
    }
}
