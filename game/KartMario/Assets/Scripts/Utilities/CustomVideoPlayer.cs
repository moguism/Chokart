using UnityEngine;
using UnityEngine.Video;

public class CustomVideoPlayer : MonoBehaviour
{
    public string videoFileName;

    public VideoPlayer videoPlayer;

    [SerializeField]
    private bool autoPlay = false;

    private string videoPath;

    private void Start()
    {
        if(autoPlay)
        {
            PlayVideo();
        }
    }

    public void PlayVideo()
    {
        videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);

        if(videoPlayer.enabled == false) { videoPlayer.enabled = true; }

        videoPlayer.url = videoPath;
        videoPlayer.Play();
    }
}
