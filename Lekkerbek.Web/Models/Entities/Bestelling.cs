using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Lekkerbek.Web.Models.Entities
{
    public class Bestelling
    {
        public enum EnumStatus
        {
            Nieuw,
            Besteld,
            Afgeleverd,
            Geannuleerd
        }

        public int Id { get; set; }

        public EnumStatus Status { get; set; } = EnumStatus.Nieuw;
        public bool Betaald { get; set; } = false;

        [Display(Name = "Klant")]
        public int KlantId { get; set; }
        public Klant Klant { get; set; }

        public DateTime? Tijdslot { get; set; }

        public string Opmerkingen { get; set; }

        public virtual ICollection<BesteldGerecht> BesteldeGerechten { get; set; }

        /// <summary>
        /// Opgelet: deze property gaat ervan uit dat BesteldeGerechten en Gerechten ook ge-Include zijn.
        /// </summary>
        [NotMapped]
        public decimal TotalePrijsZonderKorting
        {
            get
            {
                if (BesteldeGerechten == null) return 0m;
                else return BesteldeGerechten.Sum(bg => bg.Aantal * bg.Gerecht.Prijs);
            }
        }

        #region Validates en business rules

        private static Dictionary<Bestelling.EnumStatus, List<string>> _toegelatenWijzigingen = new Dictionary<Bestelling.EnumStatus, List<string>>()
        {
            { Bestelling.EnumStatus.Nieuw, new List<string> { nameof(Bestelling.Klant), nameof(Bestelling.Opmerkingen), nameof(Bestelling.BesteldeGerechten), nameof(Bestelling.Tijdslot) } },
            { Bestelling.EnumStatus.Besteld, new List<string> { nameof(Bestelling.Klant), nameof(Bestelling.Opmerkingen), nameof(Bestelling.BesteldeGerechten), nameof(Bestelling.Tijdslot), nameof(Bestelling.Betaald) } },
            { Bestelling.EnumStatus.Afgeleverd, new List<string> { nameof(Bestelling.Opmerkingen) } }
        };

        public List<string> BepaalGeldigeWijzigingen()
        {
            List<string> toegelatenWijzigingen = new List<string>();

            if (_toegelatenWijzigingen.ContainsKey(Status))
            {
                toegelatenWijzigingen = _toegelatenWijzigingen[Status].ToList(); // ToList() om niet de list in de static variabele aan te passen!

                // Wijzigingen in de 'Besteld' status kunnen maar tot 1 uur voor het tijdstlot. 
                if (Status == Bestelling.EnumStatus.Besteld
                    &&
                    Tijdslot != null
                    &&
                    Tijdslot.Value < DateTime.Now.AddHours(1))
                {
                    toegelatenWijzigingen.Clear(); // Wijzigingen mogen tot één voor het tijdslot.
                    toegelatenWijzigingen.Add(nameof(Bestelling.Betaald)); // Betalen mag altijd natuurlijk, hoe eerder hoe liever :)   
                }

                // Eénmaal de bestelling betaald is kunnen er geen wijzigingen meer doorgevoerd worden aan de bestelde gerechten.
                if (Betaald
                    &&
                    toegelatenWijzigingen.Contains(nameof(Bestelling.BesteldeGerechten)))
                {
                    toegelatenWijzigingen.Remove(nameof(Bestelling.BesteldeGerechten));
                }
            }

            return toegelatenWijzigingen;
        }

        private static Dictionary<EnumStatus, List<EnumStatus>> _toegelatenStatusovergangen = new Dictionary<EnumStatus, List<EnumStatus>>()
        {
            { EnumStatus.Nieuw, new List<EnumStatus> { EnumStatus.Besteld, EnumStatus.Geannuleerd } },
            { EnumStatus.Besteld, new List<EnumStatus> { EnumStatus.Afgeleverd, EnumStatus.Geannuleerd } },
        };

        public List<EnumStatus> BepaalGeldigeStatusovergangen()
        {
            List<EnumStatus> geldigeStatusovergangen = new List<EnumStatus>();

            if (_toegelatenStatusovergangen.ContainsKey(Status))
            {
                geldigeStatusovergangen = _toegelatenStatusovergangen[Status].ToList(); // ToList() om niet de list in de static variabele aan te passen!

                // Voor een nieuwe bestelling heeft het geen zin om 'Geannuleerd' tonen (het Tijdslot van nieuwe bestellingen is null)
                // Annuleren kan maar tot 2 uur voor het tijdslot
                if (geldigeStatusovergangen.Contains(EnumStatus.Geannuleerd)
                    &&
                    Tijdslot != null
                    &&
                    Tijdslot.Value < DateTime.Now.AddHours(2))
                {
                    geldigeStatusovergangen.Remove(EnumStatus.Geannuleerd);
                }
            }

            return geldigeStatusovergangen;
        }
        #endregion
    }
}
