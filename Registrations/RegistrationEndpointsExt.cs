namespace KeySignVerification.Registrations
{
	public static class RegistrationEndpointsExt
	{
		public static void MapRegistrationEndpoints(this WebApplication app)
		{
			// Separate your endpoints into different classes or functions for better organization
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapPost("/verifySignature", RegistrationEndpoints.VerifySignatureEndpoint);
			});
		}
	}
}
