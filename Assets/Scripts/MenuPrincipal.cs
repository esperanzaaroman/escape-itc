using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void Jugar()
    {
        // "GameScene" es el nombre que le pongas a tu escena del juego
        SceneManager.LoadScene("sampleScene");
    }

    public void Salir()
    {
        Debug.Log("Saliendo de Escape ITC...");
        Application.Quit();
    }
}
