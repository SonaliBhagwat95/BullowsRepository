using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bullows.Model
{
    public class ProposalPriceBIDViewModel
    {
        public ProposalModel ProposalDetails { get; set; }
        public PriceBIDModel PriceBIDDetails { get; set; }
        public ProposalModel MaterialList { get; set; }
    }
}
