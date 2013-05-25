using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using RangeAttribute = System.ComponentModel.DataAnnotations.RangeAttribute;

namespace Nv.CustomTypeDescriptors.Tests
{
    [TestFixture]
    public class LambdaTypeDescriptorTests
    {
        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Ctor_NullConfiguration_ThrowsException()
        {
            new LambdaTypeDescriptor(null);
        }

        [Test]
        public void GetAttributes_AddRequiredAttribute_AdditionalAttributeReturned()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModel>(); // empty configuration
            config.AddAttribute(() => new RequiredAttribute());
            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            AttributeCollection attributes = descriptor.GetAttributes();

            /* ASSERT */
            Assert.AreEqual(3, attributes.Count);

            // Serializable
            Attribute attribute = attributes[0];
            Assert.IsInstanceOf<SerializableAttribute>(attribute);

            // MetadataType
            attribute = attributes[1];
            Assert.IsInstanceOf<MetadataTypeAttribute>(attribute);
            var metadataAttribute = (MetadataTypeAttribute) attribute;
            Assert.IsTrue(typeof (TestModelWithPropertyBag) == metadataAttribute.MetadataClassType);

            attribute = attributes[2];
            Assert.IsInstanceOf<RequiredAttribute>(attribute);
        }

        [Test]
        public void GetAttributes_EmptyConfiguration_ReturnsExistingModelAttributes()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModel>(); // empty configuration
            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            AttributeCollection attributes = descriptor.GetAttributes();

            /* ASSERT */
            Assert.IsTrue(attributes.Count == 2);

            // Serializable
            Attribute attribute = attributes[0];
            Assert.IsInstanceOf<SerializableAttribute>(attribute);

            // MetadataType
            attribute = attributes[1];
            Assert.IsInstanceOf<MetadataTypeAttribute>(attribute);
            var metadataAttribute = (MetadataTypeAttribute) attribute;
            Assert.IsTrue(typeof (TestModelWithPropertyBag) == metadataAttribute.MetadataClassType);
        }

        [Test]
        public void GetAttributes_MetadataTypeAttributeInConfiguration_AttributeNotReturned()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModel>(); // empty configuration
            config.RemoveAttribute(typeof (MetadataTypeAttribute));
            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            AttributeCollection attributes = descriptor.GetAttributes();

            /* ASSERT */
            Assert.IsTrue(attributes.Count == 1);

            // Serializable
            Attribute attribute = attributes[0];
            Assert.IsInstanceOf<SerializableAttribute>(attribute);
        }

        [Test]
        public void GetProperties_AddPropertyInConfigurationOnModelWithPropertyBag_NoChangesToTypeMetadata()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModelWithPropertyBag>();
            config.AddProperty("SecondLastName", typeof (string))
                  .AddAttribute(() => new RequiredAttribute())
                  .AddAttribute(() => new StringLengthAttribute(25) {MinimumLength = 7});

            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            PropertyDescriptorCollection properties = descriptor.GetProperties();

            /* ASSERT */
            Assert.IsTrue(properties.Count == 4);

            // FirstName
            PropertyDescriptor firstName = properties[0];
            Assert.AreEqual("FirstName", firstName.Name);
            Assert.AreEqual("First name", firstName.DisplayName);

            var required = firstName.Attributes[typeof (RequiredAttribute)] as RequiredAttribute;
            Assert.IsNotNull(required);
            var displayName = firstName.Attributes[typeof (DisplayNameAttribute)] as DisplayNameAttribute;
            Assert.IsNotNull(displayName);
            Assert.AreEqual("First name", displayName.DisplayName);

            // LastName
            PropertyDescriptor lastName = properties[1];
            Assert.AreEqual("LastName", lastName.Name);

            var stringLength = lastName.Attributes[typeof (StringLengthAttribute)] as StringLengthAttribute;
            Assert.IsNotNull(stringLength);
            Assert.AreEqual(20, stringLength.MaximumLength);
            Assert.AreEqual(5, stringLength.MinimumLength);

            // Age
            PropertyDescriptor age = properties[2];
            Assert.AreEqual("Age", age.Name);

            var range = age.Attributes[typeof (RangeAttribute)] as RangeAttribute;
            Assert.IsNotNull(range);
            Assert.AreEqual(0, range.Minimum);
            Assert.AreEqual(120, range.Maximum);

            // Second last name
            PropertyDescriptor secondLastName = properties[3];
            Assert.AreEqual("SecondLastName", secondLastName.Name);

            var requiredOnSecondLastName = firstName.Attributes[typeof (RequiredAttribute)] as RequiredAttribute;
            Assert.IsNotNull(requiredOnSecondLastName);
            var stringLengthOnSecondLastName =
                secondLastName.Attributes[typeof (StringLengthAttribute)] as StringLengthAttribute;
            Assert.IsNotNull(stringLengthOnSecondLastName);
            Assert.AreEqual(25, stringLengthOnSecondLastName.MaximumLength);
            Assert.AreEqual(7, stringLengthOnSecondLastName.MinimumLength);
        }

        [Test]
        public void GetProperties_AddPropertyInConfigurationOnModelWithoutPropertyBag_NoChangesToTypeMetadata()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModel>();
            config.AddProperty("SecondLastName", typeof (string))
                  .AddAttribute(() => new RequiredAttribute())
                  .AddAttribute(() => new StringLengthAttribute(25) {MinimumLength = 7});

            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            PropertyDescriptorCollection properties = descriptor.GetProperties();

            /* ASSERT */
            Assert.IsTrue(properties.Count == 3);

            // FirstName
            PropertyDescriptor firstName = properties[0];
            Assert.AreEqual("FirstName", firstName.Name);
            Assert.AreEqual("First name", firstName.DisplayName);

            var required = firstName.Attributes[typeof (RequiredAttribute)] as RequiredAttribute;
            Assert.IsNotNull(required);
            var displayName = firstName.Attributes[typeof (DisplayNameAttribute)] as DisplayNameAttribute;
            Assert.IsNotNull(displayName);
            Assert.AreEqual("First name", displayName.DisplayName);

            // LastName
            PropertyDescriptor lastName = properties[1];
            Assert.AreEqual("LastName", lastName.Name);

            var stringLength = lastName.Attributes[typeof (StringLengthAttribute)] as StringLengthAttribute;
            Assert.IsNotNull(stringLength);
            Assert.AreEqual(20, stringLength.MaximumLength);
            Assert.AreEqual(5, stringLength.MinimumLength);

            // Age
            PropertyDescriptor age = properties[2];
            Assert.AreEqual("Age", age.Name);

            var range = age.Attributes[typeof (RangeAttribute)] as RangeAttribute;
            Assert.IsNotNull(range);
            Assert.AreEqual(0, range.Minimum);
            Assert.AreEqual(120, range.Maximum);
        }

        [Test]
        public void GetProperties_AddRequiredInConfiguration_RequiredAttachedToProperty()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModel>();
            config.ForProperty(m => m.LastName)
                  .AddAttribute(() => new RequiredAttribute());

            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            PropertyDescriptorCollection properties = descriptor.GetProperties();

            /* ASSERT */
            Assert.IsTrue(properties.Count == 3);

            // FirstName
            PropertyDescriptor firstName = properties[0];
            Assert.AreEqual("FirstName", firstName.Name);
            Assert.AreEqual("First name", firstName.DisplayName);

            var required = firstName.Attributes[typeof (RequiredAttribute)] as RequiredAttribute;
            Assert.IsNotNull(required);
            var displayName = firstName.Attributes[typeof (DisplayNameAttribute)] as DisplayNameAttribute;
            Assert.IsNotNull(displayName);
            Assert.AreEqual("First name", displayName.DisplayName);

            // LastName
            PropertyDescriptor lastName = properties[1];
            Assert.AreEqual("LastName", lastName.Name);

            var requiredOnLastName = lastName.Attributes[typeof (RequiredAttribute)] as RequiredAttribute;
            Assert.IsNotNull(requiredOnLastName);
            var stringLength = lastName.Attributes[typeof (StringLengthAttribute)] as StringLengthAttribute;
            Assert.IsNotNull(stringLength);
            Assert.AreEqual(20, stringLength.MaximumLength);
            Assert.AreEqual(5, stringLength.MinimumLength);

            // Age
            PropertyDescriptor age = properties[2];
            Assert.AreEqual("Age", age.Name);

            var range = age.Attributes[typeof (RangeAttribute)] as RangeAttribute;
            Assert.IsNotNull(range);
            Assert.AreEqual(0, range.Minimum);
            Assert.AreEqual(120, range.Maximum);
        }

        [Test]
        public void GetProperties_EmptyConfiguration_ReturnsExistingPropertiesAndAttributes()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModel>(); // empty configuration
            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            PropertyDescriptorCollection properties = descriptor.GetProperties();

            /* ASSERT */
            Assert.IsTrue(properties.Count == 3);

            // FirstName
            PropertyDescriptor firstName = properties[0];
            Assert.AreEqual("FirstName", firstName.Name);
            Assert.AreEqual("First name", firstName.DisplayName);

            var required = firstName.Attributes[typeof (RequiredAttribute)] as RequiredAttribute;
            Assert.IsNotNull(required);
            var displayName = firstName.Attributes[typeof (DisplayNameAttribute)] as DisplayNameAttribute;
            Assert.IsNotNull(displayName);
            Assert.AreEqual("First name", displayName.DisplayName);

            // LastName
            PropertyDescriptor lastName = properties[1];
            Assert.AreEqual("LastName", lastName.Name);

            var stringLength = lastName.Attributes[typeof (StringLengthAttribute)] as StringLengthAttribute;
            Assert.IsNotNull(stringLength);
            Assert.AreEqual(20, stringLength.MaximumLength);
            Assert.AreEqual(5, stringLength.MinimumLength);

            // Age
            PropertyDescriptor age = properties[2];
            Assert.AreEqual("Age", age.Name);

            var range = age.Attributes[typeof (RangeAttribute)] as RangeAttribute;
            Assert.IsNotNull(range);
            Assert.AreEqual(0, range.Minimum);
            Assert.AreEqual(120, range.Maximum);
        }

        [Test]
        public void GetProperties_RemoveFirstPropertyInConfiguration_FirstPropertyNotReturned()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModel>();
            config.ForProperty(m => m.FirstName).RemoveProperty();

            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            PropertyDescriptorCollection properties = descriptor.GetProperties();

            /* ASSERT */
            Assert.IsTrue(properties.Count == 2);

            // LastName
            PropertyDescriptor lastName = properties[0];
            Assert.AreEqual("LastName", lastName.Name);

            var stringLength = lastName.Attributes[typeof (StringLengthAttribute)] as StringLengthAttribute;
            Assert.IsNotNull(stringLength);
            Assert.AreEqual(20, stringLength.MaximumLength);
            Assert.AreEqual(5, stringLength.MinimumLength);

            // Age
            PropertyDescriptor age = properties[1];
            Assert.AreEqual("Age", age.Name);

            var range = age.Attributes[typeof (RangeAttribute)] as RangeAttribute;
            Assert.IsNotNull(range);
            Assert.AreEqual(0, range.Minimum);
            Assert.AreEqual(120, range.Maximum);
        }

        [Test]
        public void GetProperties_RemoveLastPropertyInConfiguration_LastPropertyNotReturned()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModel>();
            config.ForProperty(m => m.Age).RemoveProperty();

            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            PropertyDescriptorCollection properties = descriptor.GetProperties();

            /* ASSERT */
            Assert.IsTrue(properties.Count == 2);

            // FirstName
            PropertyDescriptor firstName = properties[0];
            Assert.AreEqual("FirstName", firstName.Name);
            Assert.AreEqual("First name", firstName.DisplayName);

            var required = firstName.Attributes[typeof (RequiredAttribute)] as RequiredAttribute;
            Assert.IsNotNull(required);
            var displayName = firstName.Attributes[typeof (DisplayNameAttribute)] as DisplayNameAttribute;
            Assert.IsNotNull(displayName);
            Assert.AreEqual("First name", displayName.DisplayName);

            // LastName
            PropertyDescriptor lastName = properties[1];
            Assert.AreEqual("LastName", lastName.Name);

            var stringLength = lastName.Attributes[typeof (StringLengthAttribute)] as StringLengthAttribute;
            Assert.IsNotNull(stringLength);
            Assert.AreEqual(20, stringLength.MaximumLength);
            Assert.AreEqual(5, stringLength.MinimumLength);
        }

        [Test]
        public void GetProperties_RemoveMiddlePropertyInConfiguration_MiddlePropertyNotReturned()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModel>();
            config.ForProperty(m => m.LastName).RemoveProperty();

            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            PropertyDescriptorCollection properties = descriptor.GetProperties();

            /* ASSERT */
            Assert.IsTrue(properties.Count == 2);

            // FirstName
            PropertyDescriptor firstName = properties[0];
            Assert.AreEqual("FirstName", firstName.Name);
            Assert.AreEqual("First name", firstName.DisplayName);

            var required = firstName.Attributes[typeof (RequiredAttribute)] as RequiredAttribute;
            Assert.IsNotNull(required);
            var displayName = firstName.Attributes[typeof (DisplayNameAttribute)] as DisplayNameAttribute;
            Assert.IsNotNull(displayName);
            Assert.AreEqual("First name", displayName.DisplayName);

            // Age
            PropertyDescriptor age = properties[1];
            Assert.AreEqual("Age", age.Name);

            var range = age.Attributes[typeof (RangeAttribute)] as RangeAttribute;
            Assert.IsNotNull(range);
            Assert.AreEqual(0, range.Minimum);
            Assert.AreEqual(120, range.Maximum);
        }

        [Test]
        public void GetProperties_RemoveRequiredAttributeInConfiguration_AttributeNotReturned()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModel>();
            config.ForProperty(m => m.FirstName).RemoveAttribute(typeof (RequiredAttribute));
            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            PropertyDescriptorCollection properties = descriptor.GetProperties();

            /* ASSERT */
            Assert.IsTrue(properties.Count == 3);

            // FirstName
            PropertyDescriptor firstName = properties[0];
            Assert.AreEqual("FirstName", firstName.Name);
            Assert.AreEqual("First name", firstName.DisplayName);

            var required = firstName.Attributes[typeof (RequiredAttribute)] as RequiredAttribute;
            Assert.IsNull(required); // SHOULD BE NULL!
            var displayName = firstName.Attributes[typeof (DisplayNameAttribute)] as DisplayNameAttribute;
            Assert.IsNotNull(displayName);
            Assert.AreEqual("First name", displayName.DisplayName);

            // LastName
            PropertyDescriptor lastName = properties[1];
            Assert.AreEqual("LastName", lastName.Name);

            var stringLength = lastName.Attributes[typeof (StringLengthAttribute)] as StringLengthAttribute;
            Assert.IsNotNull(stringLength);
            Assert.AreEqual(20, stringLength.MaximumLength);
            Assert.AreEqual(5, stringLength.MinimumLength);

            // Age
            PropertyDescriptor age = properties[2];
            Assert.AreEqual("Age", age.Name);

            var range = age.Attributes[typeof (RangeAttribute)] as RangeAttribute;
            Assert.IsNotNull(range);
            Assert.AreEqual(0, range.Minimum);
            Assert.AreEqual(120, range.Maximum);
        }

        [Test]
        public void GetProperties_ReplaceStringLengthInConfiguration_StringLenghtHasDifferentMinAndMax()
        {
            /* ARRANGE */
            var config = new LambdaDynamicTypeConfiguration<TestModel>();
            config.ForProperty(m => m.LastName)
                  .RemoveAttribute(typeof (StringLengthAttribute))
                  .AddAttribute(() => new StringLengthAttribute(30) {MinimumLength = 10});

            var descriptor = new LambdaTypeDescriptor(config);

            /* ACT */
            PropertyDescriptorCollection properties = descriptor.GetProperties();

            /* ASSERT */
            Assert.IsTrue(properties.Count == 3);

            // FirstName
            PropertyDescriptor firstName = properties[0];
            Assert.AreEqual("FirstName", firstName.Name);
            Assert.AreEqual("First name", firstName.DisplayName);

            var required = firstName.Attributes[typeof (RequiredAttribute)] as RequiredAttribute;
            Assert.IsNotNull(required);
            var displayName = firstName.Attributes[typeof (DisplayNameAttribute)] as DisplayNameAttribute;
            Assert.IsNotNull(displayName);
            Assert.AreEqual("First name", displayName.DisplayName);

            // LastName
            PropertyDescriptor lastName = properties[1];
            Assert.AreEqual("LastName", lastName.Name);

            var stringLength = lastName.Attributes[typeof (StringLengthAttribute)] as StringLengthAttribute;
            Assert.IsNotNull(stringLength);
            Assert.AreEqual(30, stringLength.MaximumLength); // DIFFERENCE HERE
            Assert.AreEqual(10, stringLength.MinimumLength); // DIFFERENCE HERE

            // Age
            PropertyDescriptor age = properties[2];
            Assert.AreEqual("Age", age.Name);

            var range = age.Attributes[typeof (RangeAttribute)] as RangeAttribute;
            Assert.IsNotNull(range);
            Assert.AreEqual(0, range.Minimum);
            Assert.AreEqual(120, range.Maximum);
        }
    }
}