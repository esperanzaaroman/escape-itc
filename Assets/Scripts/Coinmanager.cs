using UnityEngine;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{
    [Header("IA — profes en escena")]
    public List<profe> profes = new List<profe>();

    [Header("HUD")]
    public GameHUD hud;

    [Header("Puntuación")]
    public int monedasParaGanar = 5;

    // Eventos para GameManager
    public static event System.Action OnAllCoinsCollected; // jugador gana
    public static event System.Action OnProfeWins;         // profe gana

    private List<coin> _coins        = new List<coin>();
    private int        _coinIndex     = 0;
    private int        _playerScore   = 0;
    private int        _profeScore    = 0;

    public int PlayerScore => _playerScore;
    public int ProfeScore  => _profeScore;

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        foreach (coin c in FindObjectsOfType<coin>())
        {
            _coins.Add(c);
            c.gameObject.SetActive(false);
        }

        if (hud == null)
            hud = FindObjectOfType<GameHUD>();
    }

    void OnEnable()
    {
        coin.OnCoinCollectedByPlayer += HandlePlayer;
        coin.OnCoinCollectedByProfe  += HandleProfe;
    }

    void OnDisable()
    {
        coin.OnCoinCollectedByPlayer -= HandlePlayer;
        coin.OnCoinCollectedByProfe  -= HandleProfe;
    }

    void Start()
    {
        hud?.ActualizarMarcador(_playerScore, _profeScore);
        ActivarSiguiente();
    }

    // ─────────────────────────────────────────────────────────────────────────
    void ActivarSiguiente()
    {
        if (_coinIndex >= _coins.Count) return;

        coin siguiente = _coins[_coinIndex];
        siguiente.gameObject.SetActive(true);
        _coinIndex++;

        foreach (profe p in profes)
            if (p != null && p.gameObject.activeSelf)
                p.objetivo = siguiente;
    }

    void HandlePlayer(coin recogida)
    {
        _playerScore++;
        hud?.ActualizarMarcador(_playerScore, _profeScore);

        if (_playerScore >= monedasParaGanar)
        {
            OnAllCoinsCollected?.Invoke();
            return;
        }

        ActivarSiguiente();
    }

    void HandleProfe(coin recogida)
    {
        _profeScore++;
        hud?.ActualizarMarcador(_playerScore, _profeScore);

        if (_profeScore >= monedasParaGanar)
        {
            OnProfeWins?.Invoke();
            return;
        }

        ActivarSiguiente();
    }
}
