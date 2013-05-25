using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Nv.CustomTypeDescriptors
{
    public class LambdaTypeDescriptor : CustomTypeDescriptor
    {
        private readonly LambdaTypeConfiguration _configuration;

        private readonly object _lockAttributeGetter = new object();
        private readonly object _lockPropertyGetter = new object();
        private volatile AttributeCollection _finalAttributeCollection;
        private volatile PropertyDescriptorCollection _finalPropertyCollection;


        public LambdaTypeDescriptor(LambdaTypeConfiguration lambdaConfiguration)
        {
            if (lambdaConfiguration == null)
                throw new ArgumentNullException("lambdaConfiguration");

            _configuration = lambdaConfiguration;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            if (_finalPropertyCollection == null)
            {
                lock (_lockPropertyGetter)
                {
                    if (_finalPropertyCollection == null)
                    {
                        PropertyInfo[] propertyInfos =
                            TypeDescriptor.GetReflectionType(_configuration.ComponentType).GetProperties();
                        var propertiesToRemove = new HashSet<string>(_configuration.GetPropertiesToRemove());

                        var finalDescriptors = new List<PropertyDescriptor>();

                        // Handle existing properties on object
                        foreach (PropertyInfo pi in propertyInfos)
                        {
                            // Remove property
                            if (propertiesToRemove.Contains(pi.Name))
                                continue;

                            bool metadataModified;
                            Attribute[] originalAttributes = pi.GetCustomAttributes(true).Cast<Attribute>().ToArray();
                            LambdaAttributesConfiguration attributesConfiguration =
                                _configuration.GetPropertyConfiguration(pi.Name);
                            Attribute[] finalAttributes = GetAttributes(originalAttributes, attributesConfiguration,
                                                                        out metadataModified);

                            if (metadataModified)
                                finalDescriptors.Add(new LambdaPropertyDescriptor(pi.Name, pi.PropertyType,
                                                                                  finalAttributes,
                                                                                  _configuration.ComponentType));
                            else
                                finalDescriptors.Add(TypeDescriptor.CreateProperty(_configuration.ComponentType, pi.Name,
                                                                                   pi.PropertyType));
                                    // ReflectedPropertyDescriptor
                        }

                        // Handle dynamically added properties, if object implements ISimplePropertyBag
                        if (typeof (ISimplePropertyBag).IsAssignableFrom(_configuration.ComponentType))
                        {
                            IEnumerable<LambdaPropertyConfiguration> propertiesToAdd =
                                _configuration.GetPropertiesToAdd();
                            foreach (LambdaPropertyConfiguration propConf in propertiesToAdd)
                            {
                                Func<Attribute>[] attributeInitializers = propConf.GetAttributesToAdd();
                                var finalAttributes = new List<Attribute>(attributeInitializers.Length);
                                finalAttributes.AddRange(attributeInitializers.Select(func => func()));

                                finalDescriptors.Add(new LambdaPropertyDescriptor(propConf.PropertyName,
                                                                                  propConf.PropertyType,
                                                                                  finalAttributes.ToArray(),
                                                                                  _configuration.ComponentType));
                            }
                        }

                        _finalPropertyCollection = new PropertyDescriptorCollection(finalDescriptors.ToArray());
                    }
                }
            }
            return _finalPropertyCollection;
        }

        private static Attribute[] GetAttributes(Attribute[] originalAttributes,
                                                 LambdaAttributesConfiguration attributesConfiguration,
                                                 out bool metadataModified)
        {
            metadataModified = false; // default value;
            if (attributesConfiguration == null)
                return originalAttributes;

            HashSet<Type> attributesToRemove = attributesConfiguration.GetAttributesToRemove();
            Func<Attribute>[] attributesToAdd = attributesConfiguration.GetAttributesToAdd();

            if (attributesToRemove.Count == 0 && attributesToAdd.Length == 0)
                return originalAttributes;

            List<Attribute> finalAttributes;
            if (attributesToRemove.Count > 0)
            {
                finalAttributes = new List<Attribute>();
                foreach (Attribute item in originalAttributes)
                {
                    if (attributesToRemove.Contains(item.GetType()))
                    {
                        metadataModified = true;
                        continue;
                    }

                    finalAttributes.Add(item);
                }
            }
            else
                finalAttributes = new List<Attribute>(originalAttributes);

            // Add attributes to existing properties
            if (attributesToAdd.Length > 0)
            {
                foreach (var func in attributesToAdd)
                {
                    Attribute newAttr = func();
                    finalAttributes.Add(newAttr);
                    metadataModified = true;
                }
            }

            return finalAttributes.ToArray();
        }

        public override AttributeCollection GetAttributes()
        {
            if (_finalAttributeCollection == null)
            {
                lock (_lockAttributeGetter)
                {
                    if (_finalAttributeCollection == null)
                    {
                        bool metadataModified;
                        Type reflectedType = TypeDescriptor.GetReflectionType(_configuration.ComponentType);
                        Attribute[] originalAttributes =
                            reflectedType.GetCustomAttributes(true).Cast<Attribute>().ToArray();
                        Attribute[] finalAttributes = GetAttributes(originalAttributes, _configuration,
                                                                    out metadataModified);

                        _finalAttributeCollection = new AttributeCollection(finalAttributes);
                    }
                }
            }
            return _finalAttributeCollection;
        }
    }
}