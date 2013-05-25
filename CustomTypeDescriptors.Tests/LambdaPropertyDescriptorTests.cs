using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;

namespace Nv.CustomTypeDescriptors.Tests
{
    [TestFixture]
    internal class LambdaPropertyDescriptorTests
    {
        [Test]
        public void GetValue_GetsValueOfDynamicallyAddedProperty_ReturnsValue()
        {
            /* ARRANGE */
            const string propertyName = "SecondLastName";
            const string expectedPropertyValue = "2nd last name";

            var config = new LambdaDynamicTypeConfiguration<TestModelWithPropertyBag>();
            config.AddProperty(propertyName, typeof (string))
                  .AddAttribute(() => new RequiredAttribute())
                  .AddAttribute(() => new StringLengthAttribute(25) {MinimumLength = 7});

            var descriptor = new LambdaTypeDescriptor(config);
            PropertyDescriptorCollection properties = descriptor.GetProperties();
            PropertyDescriptor secondLastName = properties[3];
                // this is our internal DynamicPropertyDescriptor, visible as base class.

            var testModel = new TestModelWithPropertyBag();
            ((ISimplePropertyBag) testModel)[propertyName] = expectedPropertyValue;

            /* ACT */
            var actualPropertyValue = secondLastName.GetValue(testModel) as string;

            /* ASSERT */
            Assert.AreEqual(expectedPropertyValue, actualPropertyValue);
        }

        [Test]
        public void SetValue_SetsValueOfDynamicallyAddedProperty_StoresTheValue()
        {
            /* ARRANGE */
            const string propertyName = "SecondLastName";
            const string expectedPropertyValue = "2nd last name";

            var config = new LambdaDynamicTypeConfiguration<TestModelWithPropertyBag>();
            config.AddProperty(propertyName, typeof (string))
                  .AddAttribute(() => new RequiredAttribute())
                  .AddAttribute(() => new StringLengthAttribute(25) {MinimumLength = 7});

            var descriptor = new LambdaTypeDescriptor(config);
            PropertyDescriptorCollection properties = descriptor.GetProperties();
            PropertyDescriptor secondLastName = properties[3];
                // this is our internal DynamicPropertyDescriptor, visible as base class.

            var testModel = new TestModelWithPropertyBag();

            /* ACT */
            secondLastName.SetValue(testModel, "2nd last name");

            /* ASSERT */
            var actualPropertyValue = ((ISimplePropertyBag) testModel)[propertyName] as string;

            Assert.AreEqual(expectedPropertyValue, actualPropertyValue);
        }
    }
}