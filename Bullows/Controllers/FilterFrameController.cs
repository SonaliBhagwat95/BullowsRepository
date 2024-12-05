using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Bullows.Controllers
{
    public class FilterFrameController : BaseController
    {
        private readonly UnitOfWorks _uow;
        private readonly ISession Session;
        public FilterFrameController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;
        }
        
    }
}
