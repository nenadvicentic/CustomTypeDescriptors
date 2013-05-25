using System;

namespace Nv.CustomTypeDescriptors
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false
        , Inherited = false)]
    public sealed class LambdaTypeDescriptionAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the DynamicTypeDescriptionAttribute class.
        /// </summary>
        /// <param name="type">Type which contains function to retrive LambdaConfiguration for type.</param>
        /// <param name="lambdaConfigurationAccessorFunction">
        ///     <seealso cref="Func{LambdaDynamicTypeConfiguration}"/>
        /// </param>
        public LambdaTypeDescriptionAttribute(Type type, string lambdaConfigurationAccessorFunction)
            : this(lambdaConfigurationAccessorFunction)
        {
            Type = type;
        }

        /// <summary>
        ///     Initializes a new instance of the DynamicTypeDescriptionAttribute class.
        /// </summary>
        public LambdaTypeDescriptionAttribute(string lambdaConfigurationAccessorFunction)
        {
            LambdaConfigurationAccessorFunction = lambdaConfigurationAccessorFunction;
        }

        public Type Type { get; private set; }
        public string LambdaConfigurationAccessorFunction { get; private set; }
    }
}