using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CosmoAPITests
{
    /// <summary>
    /// This class will check if a class attribute is valid
    /// </summary>
    class ClassValidator<T>
    {
        /// <summary>
        /// This method will test the passed in model meets the validation headers inside the class definition
        /// </summary>
        /// <param name="testValue">The model to be tested</param>
        /// <returns>A list containing the error messages from the validation (will be empty if there is no errors)</returns>
        public static List<ValidationResult> Validate(T model)
        {
            List<ValidationResult> errorCollection = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, errorCollection, true);
            if(model is IValidatableObject)
            {
                ((IValidatableObject)model).Validate(validationContext);
            }
            return errorCollection;
        }
    }
}
