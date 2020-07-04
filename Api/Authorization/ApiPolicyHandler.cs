using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Authorization
{
    public class ApiPolicyHandler : AuthorizationHandler<ApiPolicyRequirement>
    {
        private readonly IHttpContextAccessor httpContext;

        //inject necessary services
        public ApiPolicyHandler(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext;
        }

        //authorization requirement logic goes here
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ApiPolicyRequirement requirement)
        {
            //can grab the user id from sub claim
            var userid = httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (userid == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
