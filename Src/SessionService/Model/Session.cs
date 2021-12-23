using System;
using System.Collections.Generic;

namespace SessionService.Model
{
    public partial class SessionModel
    {
        public Guid? SessionId { get; set; }
        public Guid MovieId { get; set; }
        public Guid RoomId { get; set; }
        public DateOnly ScreeningDate { get; set; }
        public TimeOnly StartingTime { get; set; }
        public TimeOnly EndingTime { get; set; }
    }
}
