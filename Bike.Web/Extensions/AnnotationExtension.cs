using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bike.Web.Extensions
{
    public class YearRangeTillDate: RangeAttribute
    {
        public YearRangeTillDate(int StartYear) : base(StartYear, DateTime.Today.Year)
        {

        }
    }
}
