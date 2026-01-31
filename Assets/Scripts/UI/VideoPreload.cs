using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPreload : MonoBehaviour
{
    [SerializeField]
    VideoPlayer player;

    void Start()
    {
        player.loopPointReached += Player_loopPointReached;
    }

    private void Player_loopPointReached(VideoPlayer source)
    {
        SceneManager.LoadScene("mainMenu");
    }
}
