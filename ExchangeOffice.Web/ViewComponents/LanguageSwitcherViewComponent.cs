using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ExchangeOffice.Web.ViewComponents
{
    public class LanguageSwitcherViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;
            var supportedCultures = new[] { "en", "ar" };
            
            ViewBag.CurrentCulture = currentCulture;
            ViewBag.SupportedCultures = supportedCultures;
            
            return View();
        }
    }
}
