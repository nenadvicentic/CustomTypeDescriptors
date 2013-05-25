using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Nv.CustomTypeDescriptors
{
    public sealed class LambdaDynamicTypeConfiguration<T> : LambdaTypeConfiguration
    {
        private readonly Dictionary<string, LambdaPropertyConfiguration> _added =
            new Dictionary<string, LambdaPropertyConfiguration>();

        private readonly Dictionary<string, LambdaPropertyConfiguration> _existing =
            new Dictionary<string, LambdaPropertyConfiguration>();

        public override Type ComponentType
        {
            get { return TypeDescriptor.GetReflectionType(typeof (T)); }
        }

        private static string GetPropertyName<TValue>(Expression<Func<T, TValue>> propertyAccessor)
        {
            if (propertyAccessor.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("Accessor function has to access memeber of the class.");

            var memberExpression = (MemberExpression) propertyAccessor.Body;
            string propertyName = memberExpression.Member is PropertyInfo ? memberExpression.Member.Name : null;

            return propertyName;
        }

        public LambdaPropertyConfiguration ForProperty<TValue>(Expression<Func<T, TValue>> propertyAccessor)
        {
            string propertyName = GetPropertyName(propertyAccessor);
            if (_existing.ContainsKey(propertyName))
                return _existing[propertyName];

            var propertyLambda = new LambdaPropertyConfiguration(propertyName);
            _existing.Add(propertyName, propertyLambda);
            return propertyLambda;
        }

        public LambdaPropertyConfiguration AddProperty(string propertyName, Type propertyType)
        {
            if (_added.ContainsKey(propertyName))
                return _added[propertyName];
            
            var propertyLambda = new LambdaPropertyConfiguration(propertyName, propertyType);
            _added.Add(propertyName, propertyLambda);
            return propertyLambda;
        }

        internal override IEnumerable<string> GetPropertiesToRemove()
        {
            return from propertyLambda in _existing.Values
                   where propertyLambda.SetForRemoval
                   select propertyLambda.PropertyName;
        }

        internal override IEnumerable<LambdaPropertyConfiguration> GetPropertiesToAdd()
        {
            return _added.Values;
        }

        internal override LambdaPropertyConfiguration GetPropertyConfiguration(string propertyName)
        {
            if (_existing.ContainsKey(propertyName))
                return _existing[propertyName];

            if (_added.ContainsKey(propertyName))
                return _added[propertyName];

            return null;
        }
    }

    public abstract class LambdaTypeConfiguration : LambdaAttributesConfiguration
    {
        public abstract Type ComponentType { get; }

        internal abstract IEnumerable<string> GetPropertiesToRemove();
        internal abstract IEnumerable<LambdaPropertyConfiguration> GetPropertiesToAdd();
        internal abstract LambdaPropertyConfiguration GetPropertyConfiguration(string propertyName);
    }
}