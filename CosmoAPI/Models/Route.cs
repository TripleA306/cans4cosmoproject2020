using System;
using CosmoAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CosmoAPI.Models
{
    public class Route : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int routeID { get; set; }    //Route ID, Primary Key, auto-generated when added to DB

        [Required(ErrorMessage = "Route Name cannot be empty")]
        //[MaxLength(40, ErrorMessage = "Route Name cannot be greater than 40 characters")]
        //[MinLength(2, ErrorMessage = "Route Name must be at least 2 characters")]
        public string routeName { get; set; }   //Name for the Route. Must be set and be between 2 and 40 characters long

        //A nullable foreing key used to associate a location with a region 
        [ForeignKey("region")] //specify the foreign key in location
        [Required(ErrorMessage = "Route must have a Region")]
        public int regionID { get; set; }

        //An instance of a region used for validating the foreign key 
        [Required(ErrorMessage = "Route must have a Region")]
        public Region region { get; set; }  //Region object related to the Route

        public bool completed { get; set; } //Whether the given Route has been completed

        public bool inactive { get; set; } //Whether a given Route is still considered active. Inactive routes are not retrieved by GET

        //public List<Location> locationList { get; set; } //List of locations within the route

        //[InverseProperty("Location")]
        //public List<Location> locationList { get; set; } //List of locations within the route

        //public List<LocationRoute> locationRouteList { get; set; }
        [InverseProperty("Route")]
        public List<LocationRoute> optoutLocationRouteList { get; set; } //List of locations that have opted out

        [Required]
        public DateTime routeDate { get; set; } //Date that the Route is set to run

        

        //Validation methods for the Route
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            List<ValidationResult> results = new List<ValidationResult>();  //List of validation errors

            //if (routeDate.Date < DateTime.Today)    //Checks if Date is in the past
            //{
            //    results.Add(new ValidationResult("Invalid Date", new[] { "routeDate" }));   //Adds a RouteDate error to results
            //}

            routeName = Regex.Replace(routeName, "\\s+", " ");

            if (routeName.Trim().Length < 2)
            {
                results.Add(new ValidationResult("Route Name must be at least 2 characters", new[] { "routeName" }));
            }

            if(routeName.Trim().Length > 40)
            {
                results.Add(new ValidationResult("Route Name cannot be greater than 40 characters", new[] { "routeName"}));
            }

            
            //if(routeID > 0)     //Checks if RouteID is already set
            //{
            //    results.Add(new ValidationResult("Route cannot already have an ID", new[] { "routeID" }));  //Adds a RouteID error to results
            //}

                return results;
        }

        public static explicit operator Route(string v)
        {
            throw new NotImplementedException();
        }
    }
}
