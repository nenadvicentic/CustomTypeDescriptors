namespace Nv.CustomTypeDescriptors
{
    public interface ISimplePropertyBag
    {
        object this[string key] { get; set; }
    }
}