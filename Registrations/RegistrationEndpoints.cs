using System.Text.Json;

namespace KeySignVerification.Registrations
{
    public static class RegistrationEndpoints
	{
		public static async Task VerifySignatureEndpoint(HttpContext context, RegistrationService registrationService)
		{
			var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
			var requestData = JsonSerializer.Deserialize<SignatureVerificationRequest>(requestBody);

			// You'll need to implement the following `VerifySignature` method to verify the signature
			var isSignatureValid = registrationService.VerifySignature(requestData.MessageAsBase64, requestData.SignatureAsBase64, requestData.PublicKeyAsBase64);

			var response = new { isValid = isSignatureValid };
			var jsonResponse = JsonSerializer.Serialize(response);

			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(jsonResponse);
		}
	}
}
