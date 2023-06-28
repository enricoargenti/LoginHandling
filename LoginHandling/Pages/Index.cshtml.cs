using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using LoginHandling.Models;
using System.Net.Http.Headers;
using System;
using Microsoft.AspNetCore.Identity;
using DbAccessApplication.Models;
using LoginHandling.Services;

namespace LoginHandling.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    private readonly UserManager<IdentityUser> _userManager;

    private readonly ApiProxyService _apiProxyService;

    public IndexModel(ILogger<IndexModel> logger, UserManager<IdentityUser> userManager, IHttpClientFactory httpClientFactory, ApiProxyService apiProxyService)
    {
        _logger = logger;
        _userManager = userManager;
        _apiProxyService = apiProxyService;
    }

    public async Task<IActionResult> OnPost(string userInsertedCode)
    {
        // Get the current user id
        IdentityUser currentUser = await _userManager.GetUserAsync(User);
        string currentUserId = currentUser.Id;
        Console.WriteLine("_currentUserId at the beginning: " + currentUserId);

        // Once we have the code inserited by the user,
        // we have to search for the corresponding code inside the database
        OpenDoorRequest openDoorRequest = await _apiProxyService.GetDoorOpenRequestAsync(userInsertedCode);
        if (openDoorRequest is null)
        {
            return RedirectToPage("/CodeHandling/FailedMatch");
        }

        string? deviceSentCode = openDoorRequest.DeviceGeneratedCode;
        Console.WriteLine($"Convertito a stringa: {deviceSentCode}");
        Console.WriteLine($"Codice inserito dall'utente: {userInsertedCode}");


        if (userInsertedCode.Equals(deviceSentCode))
        {
            // The first control is on permissions given to the current user to access the building

            // APIs call to get the user permissions
            UserPermissions userPermissions = await _apiProxyService.GetUserPermissionsAsync(currentUserId, openDoorRequest.DeviceId);

            if(userPermissions != null)
            {
                Console.WriteLine($"The user {userPermissions.UserName} has permissions to get inside");
                // Set the user inside the table openDoorRequests
                openDoorRequest.UserId = userPermissions.UserId;
                await _apiProxyService.UpdateOpenDoorRequestAsync(openDoorRequest.Id, openDoorRequest);
            }
            else
            {
                Console.WriteLine($"The user has no permission to get inside");

                // APIs call to delete the row (because the user request cannot be processed)
                await _apiProxyService.DeleteOpenDoorRequestAsync(openDoorRequest.Id);

                // Redirect to another page
                return base.RedirectToPage("/CodeHandling/NoPermission");
            }

            // Control that the token is still valid
            if (openDoorRequest.AccessRequestTime.AddMinutes(3) < DateTime.Now)
            {
                Console.WriteLine("Error: the code is not valid anymore");

                // APIs call to delete the expired row
                await _apiProxyService.DeleteOpenDoorRequestAsync(openDoorRequest.Id);

                // Redirect to another page
                return base.RedirectToPage("/CodeHandling/CodeExpired");
            }
            else
            {
                // Redirect to another page on successful match
                return base.RedirectToPage("/CodeHandling/SuccessfulMatch", new { code = openDoorRequest.DeviceGeneratedCode });
            }

        }
        else
        {
            // Redirect to another page on failed match
            return base.RedirectToPage("/CodeHandling/FailedMatch");
        }
    }

}