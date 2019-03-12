namespace SharpC
{
    public class ScopeVariable
    {
        public string Type;
        public string Value;

        public override string ToString()
        {
            return $"[{Type}] {Value}";
        }
    }
}