﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.OperationStatus;
using Umbraco.Cms.Web.Common.Authorization;

namespace Umbraco.Cms.Api.Management.Controllers.UserGroup;

[ApiVersion("1.0")]
public class DeleteUserGroupController : UserGroupControllerBase
{
    private readonly IUserGroupService _userGroupService;
    private readonly IAuthorizationService _authorizationService;

    public DeleteUserGroupController(IUserGroupService userGroupService, IAuthorizationService authorizationService)
    {
        _userGroupService = userGroupService;
        _authorizationService = authorizationService;
    }

    [HttpDelete("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        AuthorizationResult authorizationResult = await _authorizationService.AuthorizeAsync(User, new[] { id },
            $"New{AuthorizationPolicies.UserBelongsToUserGroupInRequest}");

        if (!authorizationResult.Succeeded)
        {
            return Forbidden();
        }

        Attempt<UserGroupOperationStatus> result = await _userGroupService.DeleteAsync(id);

        return result.Success
            ? Ok()
            : UserGroupOperationStatusResult(result.Result);
    }
}