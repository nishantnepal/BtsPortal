using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BtsPortal.Web.Infrastructure.Authorization
{
    public class PortalAuthorization
    {
        public static string GetAllowedRoles(string area, string controller, string action, PortalAuthConfigSection portalAuthSection)
        {
            if (portalAuthSection != null)
            {
                //match against action, controller and area
                foreach (var auth in portalAuthSection.PortalAuths)
                {
                    if (string.Equals(auth.Area, area, StringComparison.CurrentCulture)
                        && string.Equals(auth.Controller, controller, StringComparison.CurrentCulture)
                        && string.Equals(auth.Action, action, StringComparison.CurrentCulture)
                        )
                    {
                        return auth.AllowedRoles;
                    }
                }

                //match against controller and area
                foreach (var auth in portalAuthSection.PortalAuths)
                {
                    if (string.Equals(auth.Area, area, StringComparison.CurrentCulture)
                        && string.Equals(auth.Controller, controller, StringComparison.CurrentCulture)
                        && string.IsNullOrWhiteSpace(auth.Action)
                        )
                    {
                        return auth.AllowedRoles;
                    }
                }

                //match against area
                foreach (var auth in portalAuthSection.PortalAuths)
                {
                    if (string.Equals(auth.Area, area, StringComparison.CurrentCulture)
                        && string.IsNullOrWhiteSpace(auth.Controller) && string.IsNullOrWhiteSpace(auth.Action)
                        )
                    {
                        return auth.AllowedRoles;
                    }
                }
            }

            return string.Empty;
        }

        public static Tuple<string, string> GetAllowedRolesUsers(string area, string controller, string action, PortalAuthConfigSection portalAuthSection)
        {
            if (portalAuthSection != null)
            {
                //match against action, controller and area
                foreach (var auth in portalAuthSection.PortalAuths)
                {
                    if (string.Equals(auth.Area, area, StringComparison.CurrentCulture)
                        && string.Equals(auth.Controller, controller, StringComparison.CurrentCulture)
                        && string.Equals(auth.Action, action, StringComparison.CurrentCulture)
                        )
                    {
                        return new Tuple<string, string>(auth.AllowedRoles, auth.AllowedUsers);
                    }
                }

                //match against controller and area
                foreach (var auth in portalAuthSection.PortalAuths)
                {
                    if (string.Equals(auth.Area, area, StringComparison.CurrentCulture)
                        && string.Equals(auth.Controller, controller, StringComparison.CurrentCulture)
                        && string.IsNullOrWhiteSpace(auth.Action)
                        )
                    {
                        return new Tuple<string, string>(auth.AllowedRoles, auth.AllowedUsers);
                    }
                }

                //match against area
                foreach (var auth in portalAuthSection.PortalAuths)
                {
                    if (string.Equals(auth.Area, area, StringComparison.CurrentCulture)
                        && string.IsNullOrWhiteSpace(auth.Controller) && string.IsNullOrWhiteSpace(auth.Action)
                        )
                    {
                        return new Tuple<string, string>(auth.AllowedRoles, auth.AllowedUsers);
                    }
                }
            }

            return new Tuple<string, string>(string.Empty, string.Empty); ;
        }
    }
}