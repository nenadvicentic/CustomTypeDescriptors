using System;
using System.Collections.Generic;

namespace Nv.CustomTypeDescriptors
{
    public abstract class LambdaAttributesConfiguration
    {
        private readonly List<Func<Attribute>> _attributesToAdd = new List<Func<Attribute>>();
        private readonly HashSet<Type> _attributesToRemove = new HashSet<Type>();

        internal Func<Attribute>[] GetAttributesToAdd()
        {
            return _attributesToAdd.ToArray();
        }

        internal HashSet<Type> GetAttributesToRemove()
        {
            return _attributesToRemove;
        }

        public LambdaAttributesConfiguration AddAttribute(Func<Attribute> attributeInitializer)
        {
            _attributesToAdd.Add(attributeInitializer);

            return this;
        }

        public LambdaAttributesConfiguration AddAttributes(IEnumerable<Func<Attribute>> attributeInitializers)
        {
            _attributesToAdd.AddRange(attributeInitializers);

            return this;
        }

        public virtual LambdaAttributesConfiguration RemoveAttribute(Type attributeType)
        {
            if (!attributeType.IsSubclassOf(typeof (Attribute)))
                throw new ArgumentException("attributeType has to derive from Attribute.");

            _attributesToRemove.Add(attributeType);

            return this;
        }
    }
}