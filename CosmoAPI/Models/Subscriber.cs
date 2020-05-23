using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoAPI.Models
{
    public class Subscriber : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int subscriberID { get; set; }

        //[Required(ErrorMessage = "First Name must be between 1 and 60 characters")]
        [StringLength(60, MinimumLength = 0, ErrorMessage = "First Name must be between 0 and 60 characters")]
        [Display(Name = "First Name")]
        public string firstName { get; set; } //The users first name

        //[Required(ErrorMessage = "Last Name must be between 1 and 60 characters")]
        [StringLength(60, MinimumLength = 0, ErrorMessage = "Last Name must be between 0 and 60 characters")]
        [Display(Name = "Last Name")]
        public string lastName { get; set; } //The users last name
        
        [Remote("CheckEmail", "Validate", HttpMethod = "POST", ErrorMessage = "Email already exists")]
        [Required(ErrorMessage = "Must be a valid email address")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Must be a valid email address")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string email { get; set; } //The email address of the user

        [Required(ErrorMessage = "Phone number must be 10 digits long")]
        //[DataType(DataType.PhoneNumber)]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits long")]
        [RegularExpression(@"^([0-9]{3})?([0-9]{3})?([0-9]{4})$", ErrorMessage = "Phone number can only contain numbers")]
        [Display(Name = "Phone Number")]
        public string phoneNumber { get; set; } //The phone number of the user 


        [Display(Name = "Location ID")]
        [ForeignKey("location")] //specify the foreign key in location
        public int? locationID { get; set; } //used to link the subscriber to a location

        public Location location { get; set; }

        [Display(Name = "Billing Location")]
        [ForeignKey("billingLocation")] //specify the foreign key in location for the billing location
        public int? billingLocationID { get; set; } //The location at which Cosmo can make there receipt out too

        public Location billingLocation { get; set; }

        public bool inactive { get; set; } //specifies if the subscriber in active or inactive, (a "delete" but without removing the object)


        [Required]
        [Display(Name = "Is Buisness")]
        public bool isBusiness { get; set; } //A boolean stating if they are a buiness or not

        /// <summary>
        /// This will be used to validate our location and billinglocation IDs
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            //Check that locationID is valid
            if (location != null && locationID != location.locationID || locationID < 0)
            {
                //Its not valid so add error message
                results.Add(new ValidationResult("Must be a valid Location ID", new[] { "LocationID" }));
            }

            //Check to see if billing location id is valid
            if (billingLocation != null && billingLocationID != billingLocation.locationID || billingLocationID < 0)
            {
                //its not valid so add error message
                results.Add(new ValidationResult("Must be a valid Location ID", new[] { "BillingLocationID" }));
            }

            //return errors
            return results;
        }
		

    }
}

