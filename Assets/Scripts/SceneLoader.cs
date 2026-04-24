using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;
    public void Load() => SceneManager.LoadScene(sceneName);
}
