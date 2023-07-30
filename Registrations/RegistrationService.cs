using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeySignVerification.Registrations
{
    public class RegistrationService
    {
        public bool VerifySignature(string messageAsBase64, string signatureAsBase64, string publicKeyJsonAsBase64)
        {
            try
            {
                var messageBytes = Convert.FromBase64String(messageAsBase64);

                var signatureBytes = Convert.FromBase64String(signatureAsBase64);

                var publicKeyJsonBytes = Convert.FromBase64String(publicKeyJsonAsBase64);
                var publicKeyJson = Encoding.Default.GetString(publicKeyJsonBytes);
                // Deserialize the JSON representation of publicKey into an instance of ECDsaCngPublicKey
                var publicKey = JsonSerializer.Deserialize<ECDsaCngPublicKey>(publicKeyJson);
                // Convert Base64URL-encoded x and y values to standard Base64
                string xBase64 = Base64UrlToBase64(publicKey.X);
                string yBase64 = Base64UrlToBase64(publicKey.Y);

                var ecdsa = ECDsa.Create();

                // Import the public key parameters
                ecdsa.ImportParameters(new ECParameters
                {
                    Curve = ECCurve.NamedCurves.nistP384,
                    Q = new ECPoint
                    {
                        X = Convert.FromBase64String(xBase64),
                        Y = Convert.FromBase64String(yBase64)
                    }
                });

                // Perform the signature verification
                return ecdsa.VerifyData(messageBytes, signatureBytes, HashAlgorithmName.SHA256);
            }
            catch 
            {
                // An exception occurred during the signature verification or the publicKey is not in the correct format
                // Return false to indicate that the signature is not valid

            }
            return false;
        }

        // Helper method to convert Base64URL to standard Base64
        private static string Base64UrlToBase64(string base64Url)
        {
            string base64 = base64Url.Replace('-', '+').Replace('_', '/');
            // Add padding characters if needed
            int paddingLength = 4 - (base64.Length % 4);
            if (paddingLength < 4)
                base64 += new string('=', paddingLength);
            return base64;
        }
    }

    // Helper class for deserializing the public key JSON
    public class ECDsaCngPublicKey
    {
        [JsonPropertyName("x")]
        public string X { get; set; }

        [JsonPropertyName("y")]
        public string Y { get; set; }

        [JsonPropertyName("crv")]
        public string Crv { get; set; }
        
        [JsonPropertyName("ext")]
        public bool Ext { get; set; }

        [JsonPropertyName("key_ops")]
        public string[] KeyOps { get; set; }

        [JsonPropertyName("kty")]
        public string Kty { get; set; }
    }

}
