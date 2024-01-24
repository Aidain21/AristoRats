using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Video2 : MonoBehaviour
{

    VideoPlayer video;

    void Awake()
    {
        video = GetComponent<VideoPlayer>();
        video.Play();
        video.loopPointReached += CheckOver;


    }


    void CheckOver(UnityEngine.Video.VideoPlayer vp)
    {
        SceneManager.LoadScene("ComputerUi");//the scene that you want to load after the video has ended.
    }
}

