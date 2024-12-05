

namespace Bullows.Database
{
    public class BendSectionTable
    {
        public int BendId { get; set; }
        public decimal W { get; set; }
        public decimal H { get; set; }
        public decimal T { get; set; }
        public decimal L { get; set; }
        public decimal L1 { get; set; }
        public decimal PitchDistance { get; set; }
        //public string slotDimentions { get; set; }
        public decimal Length { get; set; }
        public string SectionName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        //public string SlotLocation { get; set; }
        public string SlotDetails { get; set; }
    }
}
