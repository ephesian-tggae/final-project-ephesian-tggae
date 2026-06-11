using MovieNest.Api.Filters;

namespace MovieNest.Api.Infrastructure;

public static class EndpointValidationExtensions
{
    public static RouteHandlerBuilder WithValidation(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilter<ValidationEndpointFilter>();
}
