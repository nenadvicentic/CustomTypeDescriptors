using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Nv.CustomTypeDescriptors
{
    public class LambdaTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly Dictionary<Type, LambdaTypeConfiguration> _configurations =
            new Dictionary<Type, LambdaTypeConfiguration>();

        private readonly HashSet<Type> _failedToLoadConfiguration = new HashSet<Type>();

        private readonly object _lockReadConfiguration = new object();

        public LambdaTypeDescriptionProvider()
        {
        }

        public LambdaTypeDescriptionProvider(TypeDescriptionProvider parent)
            : base(parent)
        {
        }

        public LambdaTypeDescriptionProvider(IEnumerable<LambdaTypeConfiguration> lambdaConfigurations)
        {
            foreach (LambdaTypeConfiguration config in lambdaConfigurations)
                _configurations.Add(config.ComponentType, config);
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            Type activeType = instance == null ? objectType : instance.GetType();

            if (_configurations.ContainsKey(activeType))
                return new LambdaTypeDescriptor(_configurations[activeType]);

            if (!_failedToLoadConfiguration.Contains(activeType))
            {
                lock (_lockReadConfiguration)
                {
                    if (!_failedToLoadConfiguration.Contains(activeType))
                    {
                        LambdaTypeConfiguration typeConfiguration = GetConfiguration(activeType);
                        if (typeConfiguration == null)
                        {
                            _failedToLoadConfiguration.Add(activeType); // optimization
                            return base.GetTypeDescriptor(objectType, instance);
                        }
                        _configurations.Add(typeConfiguration.ComponentType, typeConfiguration);
                        return new LambdaTypeDescriptor(typeConfiguration);
                    }
                    return base.GetTypeDescriptor(objectType, instance);
                }
            }
            return base.GetTypeDescriptor(objectType, instance);
        }

        private static LambdaTypeConfiguration GetConfiguration(Type objectType)
        {
            var dynamicAttribute =
                objectType.GetCustomAttributes(false).SingleOrDefault(attr => attr is LambdaTypeDescriptionAttribute) as
                LambdaTypeDescriptionAttribute;
            if (dynamicAttribute == null)
                return null;

            Type configType = dynamicAttribute.Type ?? objectType;
            const BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo mi = configType.GetMethod(dynamicAttribute.LambdaConfigurationAccessorFunction, bindingAttr, null,
                                                 Type.EmptyTypes, null);

            if (mi == null || !mi.ReturnType.IsSubclassOf(typeof (LambdaTypeConfiguration)) ||
                !mi.ReturnType.IsGenericType)
                return null;
            Type[] generics = mi.ReturnType.GetGenericArguments();
            if (generics.Length != 1 || generics[0] != objectType)
                return null;

            return (LambdaTypeConfiguration) mi.Invoke(null, null);
        }
    }
}