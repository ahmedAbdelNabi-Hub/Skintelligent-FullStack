using System.Net;

namespace SkinTelIigentContracts.CustomResponses
{
    public class ExceptionServerResponse:BaseApiResponse 
    {
        public string? details { get; set; } 
        public ExceptionServerResponse(string? massage=null, string? Details = null) : base((int)HttpStatusCode.InternalServerError, massage)
        {
           details=Details;
        }
    }
}
