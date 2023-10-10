using JiebaNet.Segmenter;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace JieBaWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class JieBaController : ControllerBase
{
    private readonly ILogger<JieBaController> _logger;
    private readonly JiebaSegmenter _segmenter;

    public JieBaController
                    (
                        ILogger<JieBaController> logger,
                        JiebaSegmenter segmenter
                    )
    {
        _logger = logger;
        _segmenter = segmenter;
    }

    [HttpPost]
    [Route("cut")]
    public async Task<IActionResult> PostAsync
                                            (
                                                [FromBody] JsonNode parameters,
                                                [FromQuery] bool cutAll = false,
                                                [FromQuery] bool hmm = true
                                            )
    {
        var text = parameters["text"]!.GetValue<string>()!;
        var segments = _segmenter.Cut(text, cutAll: cutAll, hmm: hmm);

        var r = segments
                        .All
                            (
                                (x) =>
                                {
                                    return
                                        text
                                            .Contains
                                                (
                                                    x
                                                    , StringComparison.OrdinalIgnoreCase
                                                );
                                }
                            );
        if (!r)
        {
            return
                await
                    Task
                        .FromResult
                            (
                                Conflict()
                            );


        }

        return
            await
                Task
                    .FromResult
                        (
                            Ok
                                (
                                    new
                                    {
                                        cutAll,
                                        hmm,
                                        cutted = string.Join("/ ", segments),
                                    }
                                )
                        );
    }
}