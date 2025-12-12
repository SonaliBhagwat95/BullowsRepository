using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Bullows.Controllers
{
    public class HomeController : BaseController
    {
        private readonly UnitOfWorks _uow;
        public HomeController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }
        public IActionResult Dashboard()
        {
            try
            {
                ViewBag.ActivePage = "Home";
                ViewBag.EnquiryCount = _uow.enquiryRepository.GetAllEnquiryCount();
                ViewBag.PaintBoothCount = _uow.PaintBoothRepository.GetAllPaintBoothDesignCount();
                ViewBag.RowCostingCount = _uow.costingRepository.GetCostingCountByEnquiryID();

                return View();
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("HomeController", "Dashboard", ex.Message);

                return View();

            }
        }
    }
}
