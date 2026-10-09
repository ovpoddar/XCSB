namespace Xcsb.Generators
{

    [global::System.AttributeUsage(validOn: global::System.AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class DeclarationAttribute : global::System.Attribute
    {
        public DeclarationAttribute(DeclarationKind kind) { }

        private DeclarationAttribute() { }
    }
}