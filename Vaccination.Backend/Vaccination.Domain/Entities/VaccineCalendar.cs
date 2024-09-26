using System.ComponentModel.DataAnnotations;

namespace Vaccination.Domain.Entities
{
    public class VaccineCalendar
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int MonthAge { get; set; }

        //Months after the required age to be vaccinated
        [Required]
        public int MonthDelay { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
        public bool IsDeleted { get; set; }
    }
}