using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CosmoAPI.Authorization
{
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(params string[] claim) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { claim };
        }
    }

    public class AuthorizeFilter : IAuthorizationFilter
    {
        readonly string[] _claim;

        public AuthorizeFilter(params string[] claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                bool flagClaim = false;

                for (int i = 0; i < _claim.Length; i++)
                {
                    if (context.HttpContext.User.HasClaim(_claim[i], _claim[i]))
                    {
                        flagClaim = true;
                    }
                }

                //Check for requisite role (which always MUST BE PUT IN FIRST)
                if (flagClaim)
                {
                    flagClaim = false;

                    foreach (var item in _claim)
                    {
                        if (context.HttpContext.User.HasClaim(item, item))
                        {
                            flagClaim = true;
                        }
                    }
                }

                if (!flagClaim)
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }

    }
}
