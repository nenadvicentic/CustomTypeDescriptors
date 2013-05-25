using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nv.CustomTypeDescriptors.Tests
{
    [TypeDescriptionProvider(typeof (LambdaTypeDescriptionProvider))]
    [LambdaTypeDescription("GetDynamicTypeDescription")]
    internal class TestModelWithDynamicTypeDescriptionAttribute
    {
        [DisplayName("First name")]
        [Required]
        public string FirstName { get; set; }

        [StringLength(20, MinimumLength = 5)]
        public string LastName { get; set; }

        [Range(0, 120)]
        public int Age { get; set; }

        private static LambdaDynamicTypeConfiguration<TestModelWithDynamicTypeDescriptionAttribute>
            GetDynamicTypeDescription()
        {
            var config = new LambdaDynamicTypeConfiguration<TestModelWithDynamicTypeDescriptionAttribute>();
            config.ForProperty(m => m.LastName).RemoveAttribute(typeof (StringLengthAttribute));

            return config;
        }
    }
}