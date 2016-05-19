using System;
using System.Net.Http;
using System.Net;
using PubSub;
using CallOfService.Mobile.Messages;

namespace CallOfService.Mobile
{
	public class TokenExpirationHandler : DelegatingHandler
	{
		public TokenExpirationHandler ():base(new HttpClientHandler())
		{
		}

		protected async override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
		{
			var responseMessage = await base.SendAsync (request, cancellationToken);
			if (responseMessage.StatusCode == HttpStatusCode.Unauthorized) {
				this.Publish (new UserUnauthorized());
			}
			return responseMessage;
		}
	}
}

