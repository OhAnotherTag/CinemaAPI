using FluentValidation;

namespace MovieService.Model;

public class MovieValidator : AbstractValidator<MovieModel>
{
    public MovieValidator()
    {
        RuleFor(movie => movie.Title).MinimumLength(5).MaximumLength(200);
        RuleFor(movie => movie.Plot).MinimumLength(5).MaximumLength(1000);
        RuleFor(movie => movie.Runtime).InclusiveBetween(30, 300);
        RuleFor(movie => movie.ReleaseDate).Must(BeAValidReleaseDate);
    }

    private bool BeAValidReleaseDate(DateOnly date) => date.Year >= 2021;
}