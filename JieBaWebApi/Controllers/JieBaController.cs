using JiebaNet.Segmenter;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace JieBaWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class JieBaController : ControllerBase
{
    private readonly ILogger<JieBaController> _logger;
    private readonly IConfiguration _configuration;
    private readonly JiebaSegmenter _staticSegmenter;
    private readonly JiebaSegmenter _dynamicSegmenter;

    public JieBaController
                    (
                        ILogger<JieBaController> logger,
                        IConfiguration configuration,
                        JiebaSegmenter staticSegmenter,
                        JiebaSegmenter dynamicSegmenter
                    )
    {
        _logger = logger;
        _configuration = configuration;
        _staticSegmenter = staticSegmenter;
        _dynamicSegmenter = dynamicSegmenter;
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
        var segments = _staticSegmenter.Cut(text, cutAll: cutAll, hmm: hmm);

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


    [HttpPost]
    [Route("dynamicut")]
    public async Task<IActionResult> Post2Async
                                        (
                                            [FromBody] JsonNode parameters,
                                            [FromQuery] bool cutAll = false,
                                            [FromQuery] bool hmm = true
                                        )
    {
        var text = parameters["text"]!.GetValue<string>()!;
        var segments = _dynamicSegmenter.Cut(text, cutAll: cutAll, hmm: hmm);
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

    //[HttpPost]
    [HttpPatch]
    [Route("LoadUserDict")]
    public async Task<IActionResult> Post3Async
                                        (
                                            [FromQuery] string? dict = "user_dict.txt"
                                        )
    {
        var path = Path.Combine
                            (
                                _configuration.GetValue<string>("Jieba:ResourcesFilesBaseDirectory")!
                                , dict!
                            );

        _dynamicSegmenter.LoadUserDict(path);

        return
            await
                Task
                    .FromResult
                        (
                            Ok()
                        );
    }
}