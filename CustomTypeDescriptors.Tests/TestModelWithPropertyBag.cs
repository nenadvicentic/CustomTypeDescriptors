using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nv.CustomTypeDescriptors.Tests
{
    internal class TestModelWithPropertyBag : SimplePropertyBag
    {
        [DisplayName("First name")]
        [Required]
        public string FirstName { get; set; }

        [StringLength(20, MinimumLength = 5)]
        public string LastName { get; set; }

        [Range(0, 120)]
        public int Age { get; set; }
    }
}