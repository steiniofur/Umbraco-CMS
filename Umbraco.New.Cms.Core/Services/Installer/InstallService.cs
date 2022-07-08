﻿using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.New.Cms.Core.Installer;
using Umbraco.New.Cms.Core.Models.Installer;

namespace Umbraco.New.Cms.Core.Services.Installer;

public class InstallService : IInstallService
{
    private readonly ILogger<InstallService> _logger;
    private readonly NewInstallStepCollection _installSteps;
    private readonly CustomInstallStepCollection _customInstallSteps;
    private readonly IRuntimeState _runtimeState;

    public InstallService(
        ILogger<InstallService> logger,
        NewInstallStepCollection installSteps,
        CustomInstallStepCollection customInstallSteps,
        IRuntimeState runtimeState)
    {
        _logger = logger;
        _installSteps = installSteps;
        _customInstallSteps = customInstallSteps;
        _runtimeState = runtimeState;
    }

    public async Task Install(InstallData model)
    {
        if (_runtimeState.Level != RuntimeLevel.Install)
        {
            throw new InvalidOperationException($"Runtime level must be Install to install but was: {_runtimeState.Level}");
        }

        IEnumerable<IInstallStep> steps = _installSteps.GetInstallSteps();
        await RunSteps(steps, model);

        // Run custom steps.
        foreach (ICustomInstallStep customStep in _customInstallSteps.GetInstallSteps())
        {
            ICustomInstallStepModel? stepModel = model.CustomModels.FirstOrDefault(x => x.StepKey == customStep.StepKey);
            if (stepModel is null)
            {
                _logger.LogWarning("Unable to find step model with key: {StepKey}", customStep.StepKey);
                continue;
            }

            if (await customStep.RequiresExecutionAsync(stepModel))
            {
                await customStep.ExecuteAsync(stepModel);
            }
        }
    }

    public async Task Upgrade()
    {
        if (_runtimeState.Level != RuntimeLevel.Upgrade)
        {
            throw new InvalidOperationException($"Runtime level must be Upgrade to upgrade but was: {_runtimeState.Level}");
        }

        // Need to figure out how to handle the install data, this is only needed when installing, not upgrading.
        var model = new InstallData();

        IEnumerable<IInstallStep> steps = _installSteps.GetUpgradeSteps();
        await RunSteps(steps, model);
    }

    private async Task RunSteps(IEnumerable<IInstallStep> steps, InstallData model)
    {
        foreach (IInstallStep step in steps)
        {
            var stepName = step.GetType().Name;
            _logger.LogInformation("Checking if {StepName} requires execution", stepName);
            if (await step.RequiresExecutionAsync(model) is false)
            {
                _logger.LogInformation("Skipping {StepName}", stepName);
                continue;
            }

            _logger.LogInformation("Running {StepName}", stepName);
            await step.ExecuteAsync(model);
        }
    }
}
