namespace UnitedAirlinesAPI.Models
{
    public class CargoResponse
    {
        public string Flight { get; set; }
        public string Airport { get; set; }
        public IEnumerable<Segment> Segments { get; set; }

    }

    public class Uld
    {
        public string tare_weight { get; set; }
        public string gross_weight { get; set; }
        public string breakdown_instructions { get; set; }
    }

    public class Segment
    {
        public string point_of_unlading { get; set; }
        public string point_of_lading { get; set; }
        public IEnumerable<Uld> Ulds { get; set; }
    }
}
