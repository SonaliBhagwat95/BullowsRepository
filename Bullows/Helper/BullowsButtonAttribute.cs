//using System.Reflection;
//using System.Web.Mvc;

//namespace Bullows.Helper
//{
//    [AttributeUsage(AttributeTargets.Method,AllowMultiple =false,Inherited =true)]
//    public class BullowsButtonAttribute:ActionNameSelectorAttribute
//    {
//        public string Name { get; set; }
//        public string Argument { get; set; }

//        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
//        {
//            var isValidName = false;
//            var keyValue = string.Format("{0}:{1}", Name, Argument);
//            var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);
//            if (value != null)
//            {
//                //controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
//                isValidName = true;
//            }

//            return isValidName;
//        }

//    }
//}
