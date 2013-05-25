using System;
using System.ComponentModel;
using System.Reflection;

namespace Nv.CustomTypeDescriptors
{
    internal sealed class LambdaPropertyDescriptor : PropertyDescriptor
    {
        private readonly Type _componentType;
        private readonly Type _propertyType;

        #region Constructors

        public LambdaPropertyDescriptor(string propertyName, Type propertyType, Attribute[] attrs,
                                        Type componentType = null)
            : base(propertyName, attrs)
        {
            _propertyType = propertyType;
            _componentType = componentType;
        }

        #endregion

        public override Type ComponentType
        {
            get { return _componentType; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return _propertyType; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            PropertyInfo pi = component.GetType().GetProperty(Name);
            if (pi != null)
                return pi.GetValue(component, null);

            var propertyBag = component as ISimplePropertyBag;
            if (propertyBag != null)
            {
                // PropertyBag can expose it's properties as ISimplePropertyBag
                object result = propertyBag[Name];

                if (result == null && _propertyType.IsValueType)
                    return Activator.CreateInstance(_propertyType); // ex: default value for int is 0, not null.
                return result;
            }

            throw new InvalidOperationException();
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            PropertyInfo pi = component.GetType().GetProperty(Name);
            if (pi != null)
                pi.SetValue(component, value, null);
            else
            {
                var propertyBag = component as ISimplePropertyBag;
                if (propertyBag != null)
                    // PropertyBag can expose it's properties as ISimplePropertyBag
                    propertyBag[Name] = value;
                else
                    throw new InvalidOperationException();
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            Type activeType = component.GetType();
            return activeType.IsSerializable && _propertyType.IsSerializable;
        }
    }
}