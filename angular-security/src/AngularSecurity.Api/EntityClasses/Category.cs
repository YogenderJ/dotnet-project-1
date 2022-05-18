using System.ComponentModel.DataAnnotations.Schema;

namespace AngularSecurity.Api.EntityClasses
{
    [Table("Category", Schema = "dbo")]
    public partial class Category
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
