using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    [SerializeField] string firstPlayableScene = "GameScene";

    void Start()
    {
        // Sau 1 frame để chắc chắn AudioManager Awake trước
        StartCoroutine(Go());
    }

    System.Collections.IEnumerator Go()
    {
        yield return null;
        SceneManager.LoadScene(firstPlayableScene, LoadSceneMode.Single);
    }
}
