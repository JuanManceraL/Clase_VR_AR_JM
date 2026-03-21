using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void IniciarApp(int numScene)
    {
        SceneManager.LoadScene(numScene);
    }
}
