namespace Chameleon.Web.Host.Controllers
{
    using System;
    using PayPalCheckoutSdk.Core;
    using PayPalHttp;

    using System.IO;
    using System.Text;
    using System.Runtime.Serialization.Json;
    using Chameleon.App.Payments.PayPal;
    using Abp.Dependency;

    public class PayPalClient : ITransientDependency
    {
        private readonly PayPalPaymentGatewayConfiguration _configuration;

        public PayPalClient(
            PayPalPaymentGatewayConfiguration configuration
            )
        {
            _configuration = configuration;
        }

        public string ClientId => _configuration.ClientId;

        public PayPalEnvironment environment()
        {
            if (string.Equals(_configuration.Environment, "SANDBOX", StringComparison.CurrentCultureIgnoreCase))
            {
                return new SandboxEnvironment(ClientId, _configuration.ClientSecret);
            }
            return new LiveEnvironment(ClientId, _configuration.ClientSecret);
        }

        public HttpClient client()
        {
            return new PayPalHttpClient(environment());
        }

        public HttpClient client(string refreshToken)
        {
            return new PayPalHttpClient(environment(), refreshToken);
        }

        public String ObjectToJSONString(Object serializableObject)
        {
            MemoryStream memoryStream = new MemoryStream();
            var writer = JsonReaderWriterFactory.CreateJsonWriter(
                        memoryStream, Encoding.UTF8, true, true, "  ");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(
                serializableObject.GetType(), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
            ser.WriteObject(writer, serializableObject);
            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);
            return sr.ReadToEnd();
        }
    }
}
