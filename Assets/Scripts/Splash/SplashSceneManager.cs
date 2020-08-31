using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashSceneManager : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(8);
        SceneManager.LoadSceneAsync((int)BuildSceneNames.Main);
    }
}
