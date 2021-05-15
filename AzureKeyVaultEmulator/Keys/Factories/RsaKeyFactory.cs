using System.Security.Cryptography;

namespace AzureKeyVaultEmulator.Keys.Factories
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
