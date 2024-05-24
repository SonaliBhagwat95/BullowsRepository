using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bullows.Utilities;

namespace Bullows.Controllers
{
    public class BaseController : Controller
    {
        private readonly ISession Session;
        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            this.Session = httpContextAccessor.HttpContext.Session;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (IsAuthenticated)
            {
                ViewBag.FullName = CurrentUserFullName;
                HttpContext.Session.SetString("user", CurrentUserFullName);
            }
            else
                ViewBag.FullName = "";
        }

        public void SetPanelHeading(string headerText)    
        {
            ViewBag.headerText = headerText;
            var headerTexts = String.Join("", headerText.Where(c => !char.IsWhiteSpace(c)));
            HttpContext.Session.SetString("headerText", headerTexts);
        }

        public void SetSuccessMessage(string Message)
        {
            ViewBag.SuccessText = Message;
        }

        public void SetInformationMessage(string Message)
        {
            if (!string.IsNullOrEmpty(Message))
            {
                ViewBag.InformationText = " " + Message;
            }
        }

        public void SetErrorMessage(string Message)
        {
            if (!string.IsNullOrEmpty(Message))
            {
                ViewBag.ErrorText = " " + Message;
            }
        }

        public void SetDownloadedFileNames(List<string> files)
        {
            if (files != null && files.Count > 0)
            {
                ViewBag.AllFiles = files;
            }
        }

        protected long CurrentUserId
        {
            get
            {
                // return long.Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.ClaimType.UserId).Value);
                return long.Parse(GetClaimValue(Constants.ClaimType.UserId));
            }
        }

        public string CurrentUserFullName
        {
            get
            {
                // return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.ClaimType.FullName).Value;
                return GetClaimValue(Constants.ClaimType.FullName);
            }
        }

        protected bool IsSuperAdmin
        {
            get
            {
                var principal = Thread.CurrentPrincipal;
                return principal.IsInRole(Constants.Role.SuperAdmin.ToString());
            }
        }

        protected bool IsAdmin
        {
            get
            {
                var principal = Thread.CurrentPrincipal;
                return principal.IsInRole(Constants.Role.Admin.ToString());
            }
        }

        protected bool IsGeneral
        {
            get
            {
                var principal = Thread.CurrentPrincipal;
                return principal.IsInRole(Constants.Role.General.ToString());
            }
        }

        protected bool IsAuthenticated
        {
            get
            {
                var identity = (ClaimsIdentity)HttpContext.User.Identity;//Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                if (identity != null)
                    return identity.IsAuthenticated;
                else
                    return false;
            }
        }

        private string GetClaimValue(string type)
        {

            var claimsIdentity = (ClaimsIdentity)HttpContext.User.Identity;
            //  var isAuthenticated = this._httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;       
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;// Thread.CurrentPrincipal.Identity as ClaimsIdentity;

            if (claimsIdentity != null)
            {
                var claim = claimsIdentity.FindFirst(type);

                if (claim != null)
                {
                    return claim.Value;
                }

                throw new Exception("Access denied.");
            }

            throw new Exception("Not authenticated.");
        }

        protected ActionResult RedirectToLoginPage()
        {
            return RedirectToAction("LogIn", "LogOn");
        }
    }
}
