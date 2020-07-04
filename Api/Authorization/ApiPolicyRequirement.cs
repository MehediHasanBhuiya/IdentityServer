using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Authorization
{
    public class ApiPolicyRequirement : IAuthorizationRequirement
    {
        public ApiPolicyRequirement()
        {

        }
    }
}
