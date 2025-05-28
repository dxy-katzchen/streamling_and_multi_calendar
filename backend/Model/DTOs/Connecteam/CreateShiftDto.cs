



namespace Streamling.Model.DTOs.Connecteam
{
    public class CreateShiftDto
    {



        public required int startTime { get; set; }

        public required int endTime { get; set; }

        public required string title { get; set; }

        public bool isOpenShift { get; set; } = true;

    }
}