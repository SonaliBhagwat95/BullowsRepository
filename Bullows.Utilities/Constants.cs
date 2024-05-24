using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Bullows.Utilities
{
    public static class Constants
    {
        public static class Key
        {
            public const string Pwd = "aryaman";
            public const string ModuleType = "aryaman";
            public const string ModuleID = "aryaman";
            public const string UserID = "aryaman";
        }
        public static class QueryString
        {
            public const string ExceptionId = "ExceptionId";
            public const string ModuleType = "nps";
            public const string ModuleId = "ws";
            public const string UserId = "ud";
            public const string FeedbackCategoryId = "fcd";
            public const string SignOut = "so";
            public const string UserActivation = "ud";
            public const string Token = "t";
        }

        public static class ModuleType
        {
            public const string UserActivation = "ud";
        }

        public static class ExceptionData
        {
            public const string HasLogged = "HasLogged";
            public const string ID = "ID";
        }

        public static class LookupType
        {
            public const int Title = 1;
        }

        public class Lookup
        {
            public Lookup(int id, string description)
            {
                ID = id;
                Description = description;
                Selected = false;
            }

            public Lookup(int id, string description, bool selected)
            {
                ID = id;
                Description = description;
                Selected = selected;
            }

            public int ID { get; set; }
            public string Description { get; set; }
            public bool Selected { get; set; }
        }

        public static class WorldType
        {
            public const int Country = 17;
            public const int State = 18;
            public const int City = 19;
        }

        public static class Environment
        {
            public const string DEV = "DEV";
            public const string PROD = "PROD";
        }

        public static class Session
        {
            public const string Message = "Message";
            public const string UserId = "UserId";
            public const string RedirectUrl = "RedirectUrl";
        }

        public static class Role
        {
            public const int SuperAdmin = 1;
            public const int Admin = 2;
            public const int General = 3;
        }

        public static class ClaimType
        {
            public static readonly string UserId = ClaimTypes.NameIdentifier;
            public static readonly string LoginId = "LoginId";
            public static readonly string FullName = "ClaimTypes.Name";
            public static readonly string CompanyID = "CompanyID";
            public static readonly string UserRoleID = "UserRoleID";
            public static readonly string FirstName = "FirstName";
            public static readonly string projectid = "projectid";
        }

    }
}
