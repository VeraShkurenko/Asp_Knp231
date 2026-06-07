using System.ComponentModel.DataAnnotations.Schema;

namespace AspKnP231.Data.Entities
{
    public class Discount
    {
        public Guid Id { get; set; }

        public String Title { get; set; } = null!;

        public String? Description { get; set; }

        public double Percent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        public DateTime StartMoment { get; set; }

        public DateTime? FinishMoment { get; set; }

    }
}