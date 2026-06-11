using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MovieNest.Api.Dtos;

public class SearchMoviesQuery
{
    [Required(ErrorMessage = "Query parameter q is required.")]
    [MinLength(1, ErrorMessage = "Query parameter q is required.")]
    [FromQuery(Name = "q")]
    public string? Q { get; set; }
}
