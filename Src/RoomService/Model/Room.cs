using System;
using System.Collections.Generic;
using Domain;

namespace RoomService.Model
{
    public partial class RoomModel
    {
        public Guid RoomId { get; set; }
        public int Seats { get; set; }
        public Guid CinemaId { get; set; }
        public int Number { get; set; }
        public Format Format { get; set; }
    }
}