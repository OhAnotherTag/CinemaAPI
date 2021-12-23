using System;
using System.Collections.Generic;
using Domain;
using FluentValidation;

namespace MovieService.Model
{
    public class MovieModel
    {
        public Guid MovieId { get; set; }
        public string Title { get; set; } = null!;
        public string Plot { get; set; } = null!;
        public int Runtime { get; set; }
        public Format Format { get; set; }
        public DateOnly ReleaseDate { get; set; }

        public void Validate()
        {
            var validator = new MovieValidator();
            validator.ValidateAndThrow(this);
        }
    }
}
