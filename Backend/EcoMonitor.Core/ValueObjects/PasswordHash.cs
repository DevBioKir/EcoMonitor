using EcoMonitor.Infrastracture.Abstractions;

namespace EcoMonitor.Core.ValueObjects
{
    public sealed class PasswordHash
    {
        public string Hash { get; }

        private PasswordHash(string Hash)
        {
            if (string.IsNullOrWhiteSpace(Hash))
                throw new ArgumentException("Password hash cannot be empty", nameof(Hash));

            this.Hash = Hash;
        }

        // Создание из уже захэшированного значения (например, при загрузке из БД)
        public static PasswordHash FromHash(string hash) => new PasswordHash(hash);
        // Создание из plain-текста через интерфейс хешера
        public static PasswordHash FromPlainPassword(string password, IPasswordHasher hasher)
        {
            var hash = hasher.HashPassword(password);
            return new PasswordHash(hash);
        }

        // Проверка пароля
        public bool Verify(string plainPassword, IPasswordHasher hasher)
        {
            return hasher.VerifyPassword(plainPassword, Hash);
        }

        public override string ToString() => Hash;
    }
}