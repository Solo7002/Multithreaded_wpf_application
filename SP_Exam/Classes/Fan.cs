using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_Exam.Classes
{
    internal class Fan
    {
        public int FanId { get; set; }
        public bool IsHaveTicket { get; set; }
        public int? SectorNumber { get; set; }
        public int? SeatNumber { get; set; }
        public bool IsCheckedBySecurity { get; set; }

        public Fan(int Id) 
        {
            FanId = Id;
            IsHaveTicket = false;
        }

        public Fan(int Id, int sectorNumber, int seatNumber)
        {
            FanId = Id;
            IsHaveTicket = true;
            SectorNumber = sectorNumber;
            SeatNumber = seatNumber;
        }

        public override string ToString()
        {
            return $"#{FanId}; {SectorNumber}/{SeatNumber}";
        }
    }
}
