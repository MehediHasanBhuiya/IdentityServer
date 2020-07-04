// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace OauthServer
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser
            {
                SubjectId="kjhbgjbrnougyrg87457thriuh847g",
                Username="Maruf",
                Password="password",

                Claims=new List<Claim>
                {
                    new Claim("given_name","maruf"),
                    new Claim("address","mirpur"),
                    new Claim("country","bd")
                }
            }
        };
    }
}