using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.Web.Http;

namespace PushNotificationChannelManagerWrapper
{
    public sealed class PushNotificationChannelManagerWrapper
    {
        public event Windows.Foundation.TypedEventHandler<PushNotificationChannel, PushNotificationReceivedEventArgs> PushNotificationReceived;

        PushNotificationChannel _channel = null;
        const string server = "http://Superapp.cloudapp.net/wnswrapper/api/uri";

        public async void SetupPushNotificationChannelForApplicationAsync(string token, string arguments)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("you should add you app token");
            }

            //var encryptedArguments = Helper.Encrypt(arguments, "cnblogs", "somesalt");

            _channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            var content = new HttpFormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("arguments", arguments),
                new KeyValuePair<string, string>("token", token),
                new KeyValuePair<string, string>("uri", _channel.Uri),
                new KeyValuePair<string, string>("uuid", GetUniqueDeviceId())
            });
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(server));

            request.Content = content;

            var client = new HttpClient();

            var response = await client.SendRequestAsync(request);

            _channel.PushNotificationReceived += _channel_PushNotificationReceived;
        }
        private static string GetUniqueDeviceId()
        {
            HardwareToken ht = Windows.System.Profile.HardwareIdentification.GetPackageSpecificToken(null);
            var id = ht.Id;
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(id);
            byte[] bytes = new byte[id.Length];
            dataReader.ReadBytes(bytes);
            string s = BitConverter.ToString(bytes);
            return s.Replace("-", "");
        }
        void _channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            if (PushNotificationReceived != null)
            {
                PushNotificationReceived(sender, args);
            }
        }
    }

    internal static class Helper
    {
        public static string Encrypt(string dataToEncrypt, string password, string salt)
        {
            // Generate a key and IV from the password and salt
            IBuffer aesKeyMaterial;
            IBuffer iv;
            uint iterationCount = 10000;
            GenerateKeyMaterial(password, salt, iterationCount, out aesKeyMaterial, out iv);

            IBuffer plainText = CryptographicBuffer.ConvertStringToBinary(dataToEncrypt, BinaryStringEncoding.Utf8);

            // Setup an AES key, using AES in CBC mode and applying PKCS#7 padding on the input
            SymmetricKeyAlgorithmProvider aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            CryptographicKey aesKey = aesProvider.CreateSymmetricKey(aesKeyMaterial);

            // Encrypt the data and convert it to a Base64 string
            IBuffer encrypted = CryptographicEngine.Encrypt(aesKey, plainText, iv);
            return CryptographicBuffer.EncodeToBase64String(encrypted);
        }

        public static string Decrypt(string dataToDecrypt, string password, string salt)
        {
            // Generate a key and IV from the password and salt
            IBuffer aesKeyMaterial;
            IBuffer iv;
            uint iterationCount = 10000;
            GenerateKeyMaterial(password, salt, iterationCount, out aesKeyMaterial, out iv);

            // Setup an AES key, using AES in CBC mode and applying PKCS#7 padding on the input
            SymmetricKeyAlgorithmProvider aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            CryptographicKey aesKey = aesProvider.CreateSymmetricKey(aesKeyMaterial);

            // Convert the base64 input to an IBuffer for decryption
            IBuffer ciphertext = CryptographicBuffer.DecodeFromBase64String(dataToDecrypt);

            // Decrypt the data and convert it back to a string
            IBuffer decrypted = CryptographicEngine.Decrypt(aesKey, ciphertext, iv);
            byte[] decryptedArray = decrypted.ToArray();
            return Encoding.UTF8.GetString(decryptedArray, 0, decryptedArray.Length);
        }

        private static void GenerateKeyMaterial(string password, string salt, uint iterationCount, out IBuffer keyMaterial, out IBuffer iv)
        {
            // Setup KDF parameters for the desired salt and iteration count
            IBuffer saltBuffer = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf8);
            KeyDerivationParameters kdfParameters = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, iterationCount);

            // Get a KDF provider for PBKDF2, and store the source password in a Cryptographic Key
            KeyDerivationAlgorithmProvider kdf = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha256);
            IBuffer passwordBuffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);
            CryptographicKey passwordSourceKey = kdf.CreateKey(passwordBuffer);

            // Generate key material from the source password, salt, and iteration count.  Only call DeriveKeyMaterial once,
            // since calling it twice will generate the same data for the key and IV.
            int keySize = 256 / 8;
            int ivSize = 128 / 8;
            uint totalDataNeeded = (uint)(keySize + ivSize);
            IBuffer keyAndIv = CryptographicEngine.DeriveKeyMaterial(passwordSourceKey, kdfParameters, totalDataNeeded);

            // Split the derived bytes into a seperate key and IV
            byte[] keyMaterialBytes = keyAndIv.ToArray();
            keyMaterial = WindowsRuntimeBuffer.Create(keyMaterialBytes, 0, keySize, keySize);
            iv = WindowsRuntimeBuffer.Create(keyMaterialBytes, keySize, ivSize, ivSize);
        }
    }

}
