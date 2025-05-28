using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamling.Model.DTOs.Connecteam
{
    public class UpdateShiftDto
    {
        public required string shiftId { get; set; }

        public int startTime { get; set; }

        public int endTime { get; set; }

        public string title { get; set; }

        public bool isOpenShift { get; set; } = true;
    }
}