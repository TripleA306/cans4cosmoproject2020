using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmoAPI.Models
{
    /// <summary>
    /// This class model is to represent a administrator within the system 
    /// </summary>
    public class Admin
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int adminID { get; set; } //Primary key for each admin 

        
        [Display(Name = "Username")]
        public string username { get; set; } //Unique username for each admin 

        
        [Display(Name = "Password")]
        public string password { get; set; } //Hashed password for the admin

        public byte[] salt { get; set; } //The salt that is used to hash the password 

        /// <summary>
        /// This is custom validation to make sure that the username is not already in use
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            //Call the method to check username against the context
            ValidationResult isDuplicateUsername = IsDuplicate(this, validationContext);

            //If it is a duplicate 
            if (isDuplicateUsername != null)
            {
                results.Add(isDuplicateUsername);
            }


            return results;
        }

        /// <summary>
        /// Method called by the validate method to actually check if the username exists.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        private ValidationResult IsDuplicate(object value, ValidationContext validationContext)
        {
            //Create a instance of context 
            var _cosmoContext = (CosmoContext)validationContext.GetService(typeof(CosmoContext));

            ValidationResult result = null;

            //Check the context for the username
            if (_cosmoContext.Admin.Any(e => e.username == username))
            {
                //Add the error message 
                result = new ValidationResult("Username already exists", new[] { "username" });
            }

            //return the results 
            return result;
        }
    }
}
