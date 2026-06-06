using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("Textos del HUD")]
    public TextMeshProUGUI txtMonedas;
    public TextMeshProUGUI txtTiempo;
    public TextMeshProUGUI txtProfe;

    [Header("Nombres de los profes (mismo orden que el menú)")]
    public string[] nombresProfe = { "Claudia", "Dan Perez", "Iván" };

    private float _tiempoTranscurrido = 0f;
    private bool  _corriendo          = true;

    void OnEnable()
    {
        CoinManager.OnAllCoinsCollected += DetenerTiempo;
    }

    void OnDisable()
    {
        CoinManager.OnAllCoinsCollected -= DetenerTiempo;
    }

    void Start()
    {
        int idx = GameConfig.ProfeElegido;
        if (txtProfe != null && idx < nombresProfe.Length)
            txtProfe.text = "Profe: " + nombresProfe[idx];

        ActualizarMonedas(0, 0);
    }

    void Update()
    {
        if (!_corriendo) return;
        _tiempoTranscurrido += Time.deltaTime;
        if (txtTiempo != null)
            txtTiempo.text = "Tiempo: " + FormatearTiempo(_tiempoTranscurrido);
    }

    public void ActualizarMonedas(int recogidas, int total)
    {
        if (txtMonedas != null)
            txtMonedas.text = $"Monedas: {recogidas} / {total}";
    }

    private void DetenerTiempo() => _corriendo = false;

    private string FormatearTiempo(float segundos)
    {
        int m = (int)(segundos / 60);
        int s = (int)(segundos % 60);
        return $"{m:00}:{s:00}";
    }
}