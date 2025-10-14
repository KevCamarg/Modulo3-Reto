using LibreriaDigital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Runtime;
using System.Threading;
using System.Runtime.Caching;
using System.Web.Http;
using System.Web.Http.Filters;

namespace LibreriaDigital.Filters
{
    public class RateLimitAttribute : ActionFilterAttribute
    {
        private const int MaxAttemptsByIp = 10; // Máximo intentos por IP
        private const int MaxAttemptsByEmail = 6; // Máximo intentos por email
        private static readonly TimeSpan TimeWindowByIp = TimeSpan.FromMinutes(15); // Ventana de tiempo para IP
        private static readonly TimeSpan TimeWindowByEmail = TimeSpan.FromMinutes(10); // Ventana de tiempo para email
        private static readonly MemoryCache Cache = MemoryCache.Default;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Method == HttpMethod.Options)
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            var loginRequest = actionContext.Request.Content.ReadAsAsync<LoginRequest>().Result;
            var email = actionContext.Request.Headers.Contains("X-Email") ? actionContext.Request.Headers.GetValues("X-Email").FirstOrDefault(): null;


            var ipAddress = ((HttpContextBase)actionContext.Request.Properties["MS_HttpContext"])?.Request.UserHostAddress;

            if (IsRateLimitedByIp(ipAddress) || IsRateLimitedByEmail(email))
            {
                actionContext.Response = actionContext.Request.CreateResponse((HttpStatusCode)429); // 429 = Too Many Requests
                actionContext.Response.Headers.Add("Retry-After", "60");
                return;
            }

            base.OnActionExecuting(actionContext);
        }

        private bool IsRateLimitedByIp(string ipAddress)
        {
            var cacheKey = $"RateLimit_IP_{ipAddress}";
            var attempts = Cache.Get(cacheKey) as RateLimitInfo;

            if (attempts == null)
            {
                attempts = new RateLimitInfo { Count = 1, FirstAttempt = DateTime.UtcNow };
                Cache.Set(cacheKey, attempts, DateTimeOffset.UtcNow.Add(TimeWindowByIp));
                return false;
            }

            if (DateTime.UtcNow - attempts.FirstAttempt > TimeWindowByIp)
            {
                attempts.Count = 1;
                attempts.FirstAttempt = DateTime.UtcNow;
            }
            else
            {
                attempts.Count++;
            }

            Cache.Set(cacheKey, attempts, DateTimeOffset.UtcNow.Add(TimeWindowByIp));
            return attempts.Count > MaxAttemptsByIp;
        }

        private bool IsRateLimitedByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var cacheKey = $"RateLimit_Email_{email}";
            var attempts = Cache.Get(cacheKey) as RateLimitInfo;

            if (attempts == null)
            {
                attempts = new RateLimitInfo { Count = 1, FirstAttempt = DateTime.UtcNow };
                Cache.Set(cacheKey, attempts, DateTimeOffset.UtcNow.Add(TimeWindowByEmail));
                return false;
            }

            if (DateTime.UtcNow - attempts.FirstAttempt > TimeWindowByEmail)
            {
                attempts.Count = 1;
                attempts.FirstAttempt = DateTime.UtcNow;
            }
            else
            {
                attempts.Count++;
            }

            Cache.Set(cacheKey, attempts, DateTimeOffset.UtcNow.Add(TimeWindowByEmail));
            return attempts.Count > MaxAttemptsByEmail;
        }

        private class RateLimitInfo
        {
            public int Count { get; set; }
            public DateTime FirstAttempt { get; set; }
        }
    }
}