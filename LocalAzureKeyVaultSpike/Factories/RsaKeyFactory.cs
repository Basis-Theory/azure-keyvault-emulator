using System.Security.Cryptography;

namespace LocalAzureKeyVaultSpike.Factories
{
    public static class RsaKeyFactory
    {
        private const int DefaultKeySize = 2048;

        public static RSA CreateRsaKey(int? keySize)
        {
            return RSA.Create(keySize ?? DefaultKeySize);
        }
    }
}
