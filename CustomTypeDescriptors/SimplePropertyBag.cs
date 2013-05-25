using System.Collections.Generic;

namespace Nv.CustomTypeDescriptors
{
    /// <summary>
    ///     Simple property bag. It doesn't implement any collection interfaces
    ///     in order not to clash with asp.net mvc templates engine (EditorForModel, etc).
    /// </summary>
    /// <remarks>
    ///     When LmbdaTypeDescriptor is used, inheriting from this class enables
    ///     adding properties and storing values on the fly to an object. If not used, properties can only be removed.
    /// </remarks>
    public abstract class SimplePropertyBag : ISimplePropertyBag
    {
        private readonly Dictionary<string, object> _dict = new Dictionary<string, object>();

        object ISimplePropertyBag.this[string key]
        {
            get { return _dict.ContainsKey(key) ? _dict[key] : null; }
            set { _dict[key] = value; }
        }
    }
}