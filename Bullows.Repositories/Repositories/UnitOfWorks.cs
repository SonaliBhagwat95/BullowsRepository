using System;
using System.Collections.Generic;
using System.Text;
using Bullows.Repositories.Contracts;
using System.Threading.Tasks;
using Bullows.Database;
using Microsoft.AspNetCore.Http;
using Bullows.Repositories.Repositories;

namespace Bullows.Repositories.Repositories
{
    public class UnitOfWorks : IUnitOfWork
    {
        private readonly BullowsDbContext _DbContext;

        //public UserRepository UserRepository { get; private set; }
        
        public ProjectRepository projectRepository { get; private set; }
        public UserRepository userRepository { get; private set; }
        public PanelInputRepository PanelInputRepository { get; private set; }
        public EnquiryRepository enquiryRepository { get; private set; }

        public CustomerMasterRepository CustomerMasterRepository { get; private set; }

        public PaintBoothRepository PaintBoothRepository { get; private set; }  


        public UnitOfWorks(BullowsDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _DbContext = context;
            //this.Session = httpContextAccessor.HttpContext.Session;
            
            this.userRepository = new UserRepository(this._DbContext, httpContextAccessor);          
            this.projectRepository = new ProjectRepository(this._DbContext,httpContextAccessor);
            this.PanelInputRepository = new PanelInputRepository(this._DbContext, httpContextAccessor);
            this.enquiryRepository=new EnquiryRepository(this._DbContext, httpContextAccessor);
            this .CustomerMasterRepository= new CustomerMasterRepository(this._DbContext, httpContextAccessor);
            this.PaintBoothRepository= new PaintBoothRepository(this._DbContext, httpContextAccessor);
        }
        public async Task Commit()
        {
            await this._DbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            this._DbContext.Dispose();
        }
    }
}
