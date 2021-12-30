using System;
using System.Collections.Generic;
using Domain;

namespace SessionService.Model
{
    public partial class SessionModel
    {
        public Guid SessionId { get; set; }
        public Guid MovieId { get; set; }
        public Guid RoomId { get; set; }
        public Guid CinemaId { get; set; }
        public Format MovieFormat { get; set; }
        public int MovieRuntime { get; set; }
        public Format RoomFormat { get; set; }
        public DateOnly ScreeningDate { get; set; }
        public DateOnly MovieReleaseDate { get; set; }
        public TimeOnly StartingTime { get; set; }
        public TimeOnly EndingTime { get; set; }
    }
}