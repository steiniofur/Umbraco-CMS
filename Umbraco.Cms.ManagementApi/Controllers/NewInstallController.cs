using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Infrastructure.Install;
using Umbraco.Cms.ManagementApi.Binding;
using Umbraco.Cms.ManagementApi.Filters;
using Umbraco.Cms.ManagementApi.ViewModels.Installer;
using Umbraco.Extensions;
using Umbraco.New.Cms.Core.Factories;
using Umbraco.New.Cms.Core.Installer;
using Umbraco.New.Cms.Core.Models.Installer;
using Umbraco.New.Cms.Core.Services.Installer;
using Umbraco.New.Cms.Web.Common.Routing;

namespace Umbraco.Cms.ManagementApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[BackofficeRoute("api/v{version:apiVersion}/install")]
public class NewInstallController : Controller
{
    private readonly IUmbracoMapper _mapper;
    private readonly IInstallSettingsFactory _installSettingsFactory;
    private readonly IInstallService _installService;
    private readonly GlobalSettings _globalSettings;
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly InstallHelper _installHelper;
    private readonly IJsonSerializer _serializer;
    private readonly CustomInstallStepCollection _customInstallSteps;
    private readonly ILogger<NewInstallController> _logger;

    public NewInstallController(
        IUmbracoMapper mapper,
        IInstallSettingsFactory installSettingsFactory,
        IInstallService installService,
        IOptions<GlobalSettings> globalSettings,
        IHostingEnvironment hostingEnvironment,
        InstallHelper installHelper,
        IJsonSerializer serializer,
        CustomInstallStepCollection customInstallSteps,
        ILogger<NewInstallController> logger)
    {
        _mapper = mapper;
        _installSettingsFactory = installSettingsFactory;
        _installService = installService;
        _globalSettings = globalSettings.Value;
        _hostingEnvironment = hostingEnvironment;
        _installHelper = installHelper;
        _serializer = serializer;
        _customInstallSteps = customInstallSteps;
        _logger = logger;
    }

    [HttpGet("settings")]
    [MapToApiVersion("1.0")]
    [RequireRuntimeLevel(RuntimeLevel.Install)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status428PreconditionRequired)]
    [ProducesResponseType(typeof(InstallSettingsViewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<InstallSettingsViewModel>> Settings()
    {
        // Register that the install has started
        await _installHelper.SetInstallStatusAsync(false, string.Empty);

        InstallSettingsModel installSettings = _installSettingsFactory.GetInstallSettings();
        InstallSettingsViewModel viewModel = _mapper.Map<InstallSettingsViewModel>(installSettings)!;

        return viewModel;
    }

    [HttpPost("setup")]
    [MapToApiVersion("1.0")]
    [RequireRuntimeLevel(RuntimeLevel.Install)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status428PreconditionRequired)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Setup(InstallViewModel installData)
    {
        installData.BoundCustomModels = MapToCustomInstallStepModels(installData.CustomModels).ToList();

        if (ModelState.IsValid is false)
        {
            return BadRequest(ModelState);
        }

        foreach (ICustomInstallStepModel model in installData.BoundCustomModels)
        {
            _logger.LogInformation(model.GetType().Name);
        }

        InstallData data = _mapper.Map<InstallData>(installData)!;
        await _installService.Install(data);

        var backOfficePath = _globalSettings.GetBackOfficePath(_hostingEnvironment);
        return Created(backOfficePath, null);
    }

    private IEnumerable<ICustomInstallStepModel> MapToCustomInstallStepModels(Dictionary<string, object> raw)
    {
        foreach (KeyValuePair<string, object> keyValuePair in raw)
        {
            var stepKey = Guid.Parse(keyValuePair.Key);

            ICustomInstallStep? step = _customInstallSteps.FirstOrDefault(x => x.StepKey == stepKey);
            if (step is null)
            {
                throw new InvalidOperationException($"Step not found with key: {stepKey}");
            }

            var valueString = keyValuePair.Value.ToString();
            if (valueString is null)
            {
                throw new InvalidOperationException();
            }
            Type type = step.ModelType;
            var mappedModel = JsonConvert.DeserializeObject(valueString, type);
            if (mappedModel is not null && mappedModel.GetType().Implements<ICustomInstallStepModel>())
            {
                TryValidateModel(mappedModel, mappedModel.GetType().Name);
                yield return (ICustomInstallStepModel)mappedModel;
            }
        }
    }

    [HttpPost("upgrade")]
    [MapToApiVersion("1.0")]
    [RequireRuntimeLevel(RuntimeLevel.Upgrade)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status428PreconditionRequired)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Upgrade()
    {
        await _installService.Upgrade();
        return Ok();
    }
}
