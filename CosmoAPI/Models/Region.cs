﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CosmoAPI.Models
{
    public class Region : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int regionID { get; set; }       //Region Primary Key - value is auto-generated by Context when added

        [Required(ErrorMessage ="Region name must be at least 2 characters long")]
        [MaxLength(40,ErrorMessage = "Region name cannot exceed 40 characters in length")]
        [MinLength(2, ErrorMessage = "Region name must be at least 2 characters long")]
        public string regionName { get; set; }  //String name of the region. Must be set and be between 2 and 40 characteres in length

        [Required(ErrorMessage = "Region frequency must be between 1 and 52")]
        [Range(1, 52, ErrorMessage = "Region frequency must be between 1 and 52")]  
        public int frequency { get; set; }  //Frequency in Weeks for Region locations to be picked up from. Must be set and be between 1 and 52

        [Required(ErrorMessage = "Region must have a first date set")]
        //[DateValidation(ErrorMessage = "Invalid Date")]
        public DateTime firstDate { get; set; }     //DateTime representing the first date the Region is ever picked up from. Uses the DateValidation class for some validation, must be set

        [Required(ErrorMessage = "Region must be set as Active or Inactive")]
        public bool inactive { get; set; }

        [JsonIgnore]
        [InverseProperty("Region")]
        public List<Location> locationList { get; set; } //List of locations within the region

        [JsonIgnore]
        [InverseProperty("Region")]
        public List<Route> routeList { get; set; } //List of routes for the region

        /// <summary>
        /// Generates the date for the next pick up on the route based on the current day, region's first date, and region's frequency
        /// Updates current route's routeDate once a date in the future is found
        /// 
        /// Created for potential future usage. Not current implemented
        /// </summary>
        //public DateTime GenerateNextPickupDate()
        //{
        //    DateTime nextPickup = this.firstDate;

        //    if (nextPickup < DateTime.Today)
        //    {
        //        do
        //        {
        //            nextPickup = nextPickup.AddDays(this.frequency * 7);
        //        }
        //        while (nextPickup.Date < DateTime.Today);
        //    }

        //    return nextPickup;
        //}


        /// <summary>
        /// A custom validation class for the routeDate
        /// It checks if the given date is in the past, returning false if it is, and true if it is not
        /// If this returns true the routeDate is valid, otherwise routeDate is invalid.
        /// </summary>
        //public class DateValidation : ValidationAttribute
        //{
        //    public override bool IsValid(object value)
        //    {
        //        DateTime dt = (DateTime)value;

        //        if (dt >= DateTime.Today)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //}

        //Validate method for Region object
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            //Maybe move regionID checks to POST region methods? Validate is called on PUT calls and having an ID is causing unwanted errors
            //if (regionID > 0)
            //{
            //    results.Add(new ValidationResult("Region ID cannot be set", new[] { "regionID" })); //Ensures an ID was not set for a new Region object
            //}

            //Should be placed handled in POST for regions
            //if (inactive)
            //{
            //    results.Add(new ValidationResult("Cannot create an inactive region", new[] { "inactive" }));
            //}

            return results;
        }
    }
}