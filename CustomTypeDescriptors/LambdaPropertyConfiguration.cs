using System;

namespace Nv.CustomTypeDescriptors
{
    public sealed class LambdaPropertyConfiguration : LambdaAttributesConfiguration
    {
        private readonly string _propertyName;

        private readonly Type _propertyType;

        internal LambdaPropertyConfiguration(string propertyName)
        {
            _propertyName = propertyName;
        }

        public LambdaPropertyConfiguration(string propertyName, Type propertyType)
            : this(propertyName)
        {
            DynamicallyAddedProperty = true;
            _propertyType = propertyType;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public Type PropertyType
        {
            get { return _propertyType; }
        }

        internal bool DynamicallyAddedProperty { get; private set; }
        internal bool SetForRemoval { get; private set; }

        public override LambdaAttributesConfiguration RemoveAttribute(Type attributeType)
        {
            if (DynamicallyAddedProperty)
                throw new InvalidOperationException("Removal of attributes on dynamically added property is not alowed.");

            return base.RemoveAttribute(attributeType);
        }

        public void RemoveProperty()
        {
            if (DynamicallyAddedProperty)
                throw new InvalidOperationException("Dynamically added properties should not be removed.");

            SetForRemoval = true;
        }
    }
}