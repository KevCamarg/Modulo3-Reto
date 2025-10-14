using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using LibreriaDigital.Services;
using System.Security.Claims;

namespace LibreriaDigital.Handler
{
    public class JwtAuthenticationHandler : DelegatingHandler
    {
        private readonly JwtTokenService _jwtTokenService;

        public JwtAuthenticationHandler()
        {
            _jwtTokenService = new JwtTokenService();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization != null)
            {
                var token = request.Headers.Authorization.Parameter;
                var principal = _jwtTokenService.ValidateToken(token);

                if (principal != null)
                {
                    // Establecer el usuario en el contexto
                    var identity = principal.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        request.GetRequestContext().Principal = principal;
                    }
                }
                else
                {
                    return request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}