using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Panel de Victoria")]
    public GameObject panelVictoria;
    public TextMeshProUGUI txtTiempoFinal;
    public Button btnMenuVictoria;

    [Header("Panel de Derrota")]
    public GameObject panelDerrota;
    public Button btnReintentar;
    public Button btnMenuDerrota;

    [Header("Referencias")]
    public GameHUD hud;

    private bool _juegoTerminado = false;

    // ─────────────────────────────────────────────────────────────────────────
    void OnEnable()
    {
        CoinManager.OnAllCoinsCollected += MostrarVictoria;
    }

    void OnDisable()
    {
        CoinManager.OnAllCoinsCollected -= MostrarVictoria;
    }

    void Start()
    {
        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (panelDerrota  != null) panelDerrota.SetActive(false);

        btnMenuVictoria?.onClick.AddListener(VolverAlMenu);
        btnReintentar  ?.onClick.AddListener(Reintentar);
        btnMenuDerrota ?.onClick.AddListener(VolverAlMenu);

        // Liberar cursor al terminar
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = false;
    }

    // ── Victoria ──────────────────────────────────────────────────────────────
    void MostrarVictoria()
    {
        if (_juegoTerminado) return;
        _juegoTerminado = true;

        // Pausar juego
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        // Mostrar tiempo final del HUD
        if (txtTiempoFinal != null && hud != null)
            txtTiempoFinal.text = "Tiempo: " + hud.GetTiempoFormateado();

        if (panelVictoria != null) panelVictoria.SetActive(true);
    }

    // ── Derrota (llamada desde profe.cs cuando atrapa al jugador) ─────────────
    public void MostrarDerrota()
    {
        if (_juegoTerminado) return;
        _juegoTerminado = true;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        if (panelDerrota != null) panelDerrota.SetActive(true);
    }

    // ── Navegación ────────────────────────────────────────────────────────────
    void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("menuScene");
    }

    void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }
}
