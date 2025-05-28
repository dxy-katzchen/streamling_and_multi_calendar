

namespace Streamling.Model.DTOs.Calendar
{
    public class SchedulerLabelDto
    {
        public string icon { get; } = "";
        //property name
        public required string title { get; set; }
        //platform name - channel name
        public required string subtitle { get; set; }
    }
}