using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lekkerbek.Web.Models.Entities
{
    public class Gerecht
    {
        public int Id { get; set; }

        [Required]
        public string Categorie { get; set; }
        [Required]
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public decimal Prijs { get; set; }
    }
}
