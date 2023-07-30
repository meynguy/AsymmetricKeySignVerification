using System.Text.Json.Serialization;

namespace KeySignVerification.Registrations
{
    public class SignatureVerificationRequest
    {
        [JsonPropertyName("messageAsBase64")]
        public string MessageAsBase64 { get; set; }
        
        [JsonPropertyName("signatureAsBase64")]
        public string SignatureAsBase64 { get; set; }

        [JsonPropertyName("publicKeyAsBase64")]
        public string PublicKeyAsBase64 { get; set; }
    }

}
