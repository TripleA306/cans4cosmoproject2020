using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoAPI.Models
{
    /// <summary>
    /// This object will be used to store information about a certain location
    /// It is used as a Billing address and a pickUpAddress
    /// </summary>
    public class Location : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int locationID { get; set; } //An integer to uniquely identify each instance of locaiton

        //An instance of a region used for validating the foreign key 
        public Region region { get; set; }

        //A nullable foreing key used to associate a location with a region 
        [ForeignKey("Region")] //specify the foreign key in location
        public int? regionID { get; set; } 

        [Required(ErrorMessage = "City is required")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "City must contain at least two and no more than 60 characters")]
        [Display(Name = "City")]
        public string city { get; set; } //a string containing the city that accosiated with the location

        [StringLength(15, ErrorMessage = "Unit/Apt. cannot exceed 15 characters in length" )]
        [Display(Name = "Unit")]
        [RegularExpression(@"^[A-Za-z0-9-]+$", ErrorMessage = "Unit/Apt. can only contain letters, numbers, and the - symbol")]
        public string unit { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Street address must be between 2 and 60 characters")]
        [Display(Name = "Address")]
        public string address { get; set; } //This is the street that the location is associated with

        //https://stackoverflow.com/questions/1146202/canadian-postal-code-validation
        [Required(ErrorMessage = "Postal Code is required")]
        [RegularExpression(@"[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ] [0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]", ErrorMessage = "Postal code must be in the format A1A 1A1")]
        public string postalCode { get; set; }// This is the locations postal code

        public bool inactive = false;

        [Required]
        public string province { get; set; } // The province associated with the location object

        [Required]
        public string locationType { get; set; } //This Will store whether it is a billing location or an pick up location
        [InverseProperty("Location")]
        public List<LocationRoute> optoutLocationRouteList { get; set; }

        //[InverseProperty("Route")]
        //public List<Route> routeList { get; set; } //List of routes that the location has been associated with

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            List<string> provs = new List<string>();
            provs.Add("ON");
            provs.Add("QC");
            provs.Add("NS");
            provs.Add("NB");
            provs.Add("MB");
            provs.Add("BC");
            provs.Add("PE");
            provs.Add("SK");
            provs.Add("AB");
            provs.Add("NL");
            provs.Add("NT");
            provs.Add("YT");
            provs.Add("NU");

            List<string> values = new List<string>();
            values.Add("billing");
            values.Add("pickup");


            //if (!provs.Contains(Province.ToLower()))
            //{
            //    results.Add(new ValidationResult("Please enter a province or territory with its full name", new[] { "Province" }));

            //}

            if (!values.Contains(locationType.ToLower()))
            {
                results.Add(new ValidationResult("Error: Invalid pickup type", new[] { "LocationType" }));
            }

            //if (regionID != Region.regionID)
            //{
            //    results.Add(new ValidationResult("The entered region does not exist", new[] { "regionID" }));
            //}

            return results;
        }
    }

}