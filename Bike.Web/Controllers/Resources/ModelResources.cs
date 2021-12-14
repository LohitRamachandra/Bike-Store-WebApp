using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bike.Web.Controllers.Resources
{
    public class ModelResources
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int MakeID { get; set; }
    }
}
