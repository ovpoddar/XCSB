namespace Xcsb.Generators
{
    [global::System.AttributeUsage(validOn: global::System.AttributeTargets.Class, AllowMultiple = false,
        Inherited = false)]
    public class ImplementationAttribute : global::System.Attribute
    {
        public ImplementationAttribute(ImplementationKind kind, global::System.Type target) { }

        private ImplementationAttribute() { }
    }
}