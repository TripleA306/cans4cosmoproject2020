using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoAPI.Models
{
    public class LocationRoute
    {
        //Location Route object with the Location and Route object inside. Both ID's are foreign keys
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int locationRouteID { get; set; }
        
        [ForeignKey("Location")]
        public int locationID { get; set; }
        public Location location { get; set; }

        [ForeignKey("Route")]
        public int routeID { get; set; }
        public Route route { get; set; }
    }
}