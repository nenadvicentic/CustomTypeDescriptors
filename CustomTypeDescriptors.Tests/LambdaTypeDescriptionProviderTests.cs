using System.ComponentModel;
using NUnit.Framework;

namespace Nv.CustomTypeDescriptors.Tests
{
    [TestFixture]
    internal class LambdaTypeDescriptionProviderTests
    {
        [Test]
        public void GetTypeDescriptor_LambdaTypeDescriptionAttributeSet_ReturnsLambdaDynamicTypeConfiguration()
        {
            var provider = new LambdaTypeDescriptionProvider(); // empty provider, without configurations

            /* ACT */
            ICustomTypeDescriptor descriptor =
                provider.GetTypeDescriptor(typeof (TestModelWithDynamicTypeDescriptionAttribute));

            /* ASSERT */
            Assert.IsNotNull(descriptor);
            PropertyDescriptorCollection properties = descriptor.GetProperties();
            Assert.AreEqual(0, properties[1].Attributes.Count);
        }

        [Test]
        public void
            GetTypeDescriptor_TypeDescriptionProviderAttributeSetAndLambdaTypeDescriptionAttributeSet_ReturnsDynamicTypeDescription
            ()
        {
            /* ARRANGE */
            var instance = new TestModelWithDynamicTypeDescriptionAttribute();

            /* ACT */
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);
                // TypeDescriptor triggers DynamicTypeDescriptionProvider because of attribute on our object instance.

            /* ASSERT */
            Assert.AreEqual(0, properties[1].Attributes.Count);
        }
    }
}