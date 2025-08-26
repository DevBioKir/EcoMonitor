using System.Text.RegularExpressions;

namespace EcoMonitor.Core.ValueObjects
{
    public sealed class Email : IEquatable<Email>
    {
        private static readonly Regex _regex = 
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public string Value { get; }

        private Email(string Value)
        {
            this.Value = Value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) 
                throw new ArgumentException("Email cannot be empty", nameof(value));

            if (!_regex.IsMatch(value)) 
                throw new ArgumentException("Invalid email format", nameof(value));

            return new Email(value.Trim().ToLowerInvariant());
        }

        public bool Equals(Email? other)
        {
            if (other is null) return false;
            return Value == other.Value;
        }

        public override bool Equals(object? obj) => Equals(obj as Email);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
    }
}
