using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lekkerbek.Web.Models.Entities
{
    public class Korting
    {
        public int Id { get; set; }

        public int AantalBestellingen { get; set; }
        public decimal Percentage { get; set; }
    }
}
