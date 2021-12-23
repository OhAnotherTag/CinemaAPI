using System;
using System.Collections.Generic;

namespace CinemaService.Model
{
    public partial class CinemaModel
    {
        public Guid CinemaId { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
    }
}
