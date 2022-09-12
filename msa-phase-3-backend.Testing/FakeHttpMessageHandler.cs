using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msa_phase_3_backend.Testing
{
    // Used for mocking HTTPClient
    // https://anthonygiretti.com/2018/09/06/how-to-unit-test-a-class-that-consumes-an-httpclient-with-ihttpclientfactory-in-asp-net-core/
    public class FakeHttpMessageHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage _fakeResponse;

        public FakeHttpMessageHandler(HttpResponseMessage responseMessage)
        {
            _fakeResponse = responseMessage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_fakeResponse);
        }
    }
}
