using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bullows.Model
{
    public class ProjectModel : BaseModel
    {
        #region ProjectModel
       
        public int ProjectID { get; set; }
        [Required(ErrorMessage = "*")]
        public string ProjectCode { get; set; }
        [Required(ErrorMessage = "*")]
        public string ProjectName { get; set; }
        [Required(ErrorMessage = "*")]
        public string CustomerName { get; set; }
        #endregion
    }
}
