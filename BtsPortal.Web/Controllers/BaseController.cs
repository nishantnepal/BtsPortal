using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BtsPortal.Web.Infrastructure;
using BtsPortal.Web.Infrastructure.Authorization;

namespace BtsPortal.Web.Controllers
{
    [PortalAuthorize]
    public abstract class BaseController : Controller
    {
        
    }
}