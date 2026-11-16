using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Figures;

namespace Portfolio.Api.Controllers;

/// <summary>The figures (diagrams and 3D) a post can embed with ::figure{id=...}.</summary>
[ApiController]
[Route("api/figures")]
[Produces("application/json")]
public sealed class FiguresController : ControllerBase
{
    /// <summary>Lists available figure ids with a description of each.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<FigureInfo>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<FigureInfo>> GetAll() => Ok(FigureCatalog.All);
}
