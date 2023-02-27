using System.ComponentModel.DataAnnotations;
using EDBlog.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using EDBlog.Domain.Contracts;
using MassTransit;

namespace EDBlog.WebAPI.Controllers;

public class GetAuthorController : Controller
{
    private readonly IMediator mediator;

    public GetAuthorController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet, Route("author/{authorId}")]
    public Task<IResult> GET(Guid authorId)
    {
        throw new NotImplementedException();
    }
}