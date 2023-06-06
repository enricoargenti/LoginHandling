using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LoginHandling.Services;

namespace LoginHandling.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private string _userInsertedCode { get; set; }

    // Fields useful to check if the code is on the queue on the IoT Hub
    QueueListener queueListener;
    public string _deviceSentCode { get; private set; }
    public string _deviceId { get; private set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }

    public IActionResult OnPostCodeMatcher()
    {
        // Access the form input value
        _userInsertedCode = Request.Form["userInsertedCode"];

        // Once we have the code inserited by the user,
        // we have to search for the corresponding enqueued code on the IoT Hub
        // Check if the code is on the queue
        queueListener = new QueueListener();
        queueListener.ListenQueue().Wait();
        _deviceSentCode = queueListener.ReceivedMessage;
        _deviceId = queueListener.DeviceId;

        if(_deviceSentCode == "no-message")
        {
            return RedirectToPage("/CodeHandling/NoMessageAvailable", new { DeviceId = _deviceId });
        }

        if (_userInsertedCode == _deviceSentCode)
        {
            // Redirect to another page on successful match
            return RedirectToPage("/CodeHandling/SuccessfulMatch", new { DeviceId = _deviceId });
        }
        else
        {
            // Redirect to another page also on failed match
            return RedirectToPage("/CodeHandling/FailedMatch", new { DeviceId = _deviceId });
        }
    }

}