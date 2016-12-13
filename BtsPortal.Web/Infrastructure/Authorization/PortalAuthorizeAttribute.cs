using System.Web.Mvc;
using BtsPortal.Web.Infrastructure.Settings;

namespace BtsPortal.Web.Infrastructure.Authorization
{
    public class PortalAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var rd = filterContext.HttpContext.Request.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");
            string currentArea = string.Empty;
            object oCurrentArea;
            if (rd.DataTokens.TryGetValue("area", out oCurrentArea))
            {
                currentArea = (oCurrentArea as string);
            }

            var portalAuthSection = AppSettings.PortalAuth;
            //string allowedRoles = PortalAuthorization.GetAllowedRoles(currentArea, currentController, currentAction, portalAuthSection);
            var allowedRolesUsers = PortalAuthorization.GetAllowedRolesUsers(currentArea, currentController, currentAction, portalAuthSection);
            string allowedRoles = allowedRolesUsers.Item1;
            string allowedUsers = allowedRolesUsers.Item2;
            if (string.IsNullOrWhiteSpace(allowedRoles)&& string.IsNullOrWhiteSpace(allowedUsers))
            {
                string error =
                    $"No Roles or Users defined in configuration for {currentArea}/{currentController}/{currentAction}";
                filterContext.Result = new RedirectResult("~/Error/AccessDenied?errorDesc=" + error);
            }
            else
            {
                Roles = allowedRoles;
                Users = allowedUsers;
                //Users
                //base.OnAuthorization(filterContext);
                bool isAjaxReq =
                    filterContext.RequestContext.HttpContext.Request.IsAjaxRequest();

                if (filterContext.Result is HttpUnauthorizedResult)
                {
                    filterContext.Result = !isAjaxReq
                        ? new RedirectResult("~/Error/AccessDenied?roles=" + Roles)
                        : new RedirectResult("~/Error/AccessDeniedPartial?roles=" + Roles);

                }
            }

        }
    }
}