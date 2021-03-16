using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lekkerbek.Web.Models.Entities
{
    public class Klant
    {
        public int Id { get; set; }

        [Required]
        public string Naam { get; set; }

        [Display(Name = "Straat en nummer")]
        public string StraatEnNummer { get; set; }

        [Range(0, 9999)]
        public int Postcode { get; set; }

        public string Woonplaats { get; set; }

        public string Telefoonnummer { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Opmerkingen { get; set; }

        public virtual ICollection<Bestelling> Bestellingen { get; set; }
    }
}
