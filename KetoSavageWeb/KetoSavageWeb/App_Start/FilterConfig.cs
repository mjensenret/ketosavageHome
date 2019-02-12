using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new AuthorizeAttribute());
        }
    }
}