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
                filePath = "https://drive.google.com/file/d/14lf-4ZT-Qf4xJeXH-FEeca4lPoCcAoi8/view?usp=drive_link";
                break;
                //windows
            case "exe":
                filePath = "https://drive.google.com/file/d/1sudF6Xhur9MEJ69wgiL80zesfBopPxe2/view?usp=sharing";
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
