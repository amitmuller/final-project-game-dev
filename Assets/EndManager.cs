using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public PlayableDirector director;

    void Start()
    {
        if (director != null)
        {
            director.stopped += OnTimelineStopped;
        }
    }

    void OnTimelineStopped(PlayableDirector pd)
    {
        if (pd == director)
        {
            SceneManager.LoadScene("START_SCENE");
        }
    }
}
