

namespace Streamling.Model.DTOs.Calendar
{
    public class SchedulerDto
    {
        //property id
        public required string id { get; set; }

        public required SchedulerLabelDto label { get; set; }

        public required List<SchedulerDataDto> data { get; set; }
    }
}