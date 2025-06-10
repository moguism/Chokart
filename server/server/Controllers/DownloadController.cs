using Microsoft.AspNetCore.Mvc;

namespace server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DownloadController : ControllerBase
{
    // [Authorize] // si queremos que este logueado
    [HttpGet("{platform}")]
    public IActionResult DownloadFile(string platform)
    {
        string filePath;

        switch (platform.ToLower())
        {
                //android
            case "apk":
                filePath = "https://drive.google.com/file/d/1iGe3Gg7AI_XUKxA065Es9DHiaJjRjNJS/view?usp=sharing";
                break;
                //windows
            case "exe":
                filePath = "https://drive.google.com/file/d/1PZiMjDM3grZOa7gQ3K1zsNVvIbe3o5mg/view?usp=drive_link";
                break;
                //iphone
            case "ipa":
                filePath = "https://drive.google.com/file/d/1QPa6MJn_6-ZByqLnS80zHBhvlGxYQMIG/view?usp=drive_link";
                break;
                //mac
            case "app":
                filePath = "https://drive.google.com/file/d/1gEQ8yzYPZegIVNhBto1XfeeeZ5B4gPpl/view?usp=drive_link";
                break;
            case "linux":
                filePath = "https://drive.google.com/file/d/1gaD-xKQMGF3pm3r2JoLNHOphXCfTKzXr/view?usp=drive_link";
                break;
            default:
                return null;
        }

        return Ok(filePath);
    }
}
