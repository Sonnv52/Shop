using System.ComponentModel.DataAnnotations;

namespace Shop.Api.Models.Page
{
    public class PageQuery
    {
        public int pageIndex { get; set; }  
        public int pageSize { get; set; }

        [RegularExpression(@"^(0?[1-9]|[12][0-9]|3[01])\-(0?[1-9]|1[0-2])\-\d{4}$", ErrorMessage = "Invalid date format. Please use format dd-MM-yyyy")]
        public string? StartDate { get; set; }
        [RegularExpression(@"^(0?[1-9]|[12][0-9]|3[01])\-(0?[1-9]|1[0-2])\-\d{4}$", ErrorMessage = "Invalid date format. Please use format dd-MM-yyyy")]
        public string? EndDate { get; set; }
        public bool? InDay { get; set; }
        public bool? InMonth { get; set;}
        public bool? InYear { get; set;}
    } 
}
