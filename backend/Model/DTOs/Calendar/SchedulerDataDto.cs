

namespace Streamling.Model.DTOs.Calendar
{
    public class SchedulerDataDto
    {
        //reservation id
        public required string id { get; set; }

        public required string startDate { get; set; }

        public required string endDate { get; set; }

        public int occupancy { get; } = 0;
        //property name
        public required string title { get; set; }
        //platform name - channel name
        public required string subtitle { get; set; }
        //hostnote
        public required string description { get; set; }

        public required string bgColor { get; set; }
    }
}