using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuPrincipal : MonoBehaviour
{
    public void Jugar()
    {
        MostrarPanelSeleccion();
    }

    public void Salir()
    {
        Debug.Log("Saliendo de Escape ITC...");
        Application.Quit();
    }

    [Header("Botones del menú principal")]
    public GameObject botonIniciar;   
    public GameObject botonSalir;     

    // ── Selección de profe ────────────────────────────────────────────────────
    [Header("Panel de selección de profe")]
    public GameObject panelSeleccion;

    public Button btnProfe0;
    public Button btnProfe1;
    public Button btnProfe2;

    public Image           imgPreview;
    public TextMeshProUGUI txtNombreProfe;
    public TextMeshProUGUI txtDescProfe;
    public TextMeshProUGUI txtAviso;
    public Button          botonJugar;

    [Header("Datos de los 3 profes")]
    public ProfeData[] profes = new ProfeData[3];

    private int _profeSeleccionado = -1;

    private static readonly Color COL_NORMAL    = Color.white;
    private static readonly Color COL_SELECCION = new Color(0.3f, 0.75f, 1f);

    void Start()
    {
        // Panel oculto al inicio, botones del menú visibles
        if (panelSeleccion != null) panelSeleccion.SetActive(false);
        if (botonJugar     != null) botonJugar.gameObject.SetActive(false);
        if (txtAviso       != null) txtAviso.text = "Elige un profe para continuar";

        btnProfe0?.onClick.AddListener(() => SeleccionarProfe(0));
        btnProfe1?.onClick.AddListener(() => SeleccionarProfe(1));
        btnProfe2?.onClick.AddListener(() => SeleccionarProfe(2));
        botonJugar?.onClick.AddListener(ConfirmarYJugar);
    }

    void MostrarPanelSeleccion()
    {
        // Ocultar botones del menú principal
        if (botonIniciar != null) botonIniciar.SetActive(false);
        if (botonSalir   != null) botonSalir.SetActive(false);

        // Mostrar panel de selección
        if (panelSeleccion != null) panelSeleccion.SetActive(true);
    }

    void SeleccionarProfe(int index)
    {
        _profeSeleccionado      = index;
        GameConfig.ProfeElegido = index;

        if (index < profes.Length && profes[index] != null)
        {
            ProfeData d = profes[index];
            if (txtNombreProfe != null) txtNombreProfe.text = d.nombre;
            if (txtDescProfe   != null) txtDescProfe.text   =
                $"Velocidad: {d.velocidad}  |  Memoria: {d.capacidadMemoria} nodos\n{d.descripcion}";
        }

        ResetBotonesColor();
        SetButtonColor(GetBoton(index), COL_SELECCION);

        if (botonJugar != null) botonJugar.gameObject.SetActive(true);
        if (txtAviso   != null) txtAviso.gameObject.SetActive(false);
    }

    void ConfirmarYJugar()
    {
        if (_profeSeleccionado < 0) return;
        SceneManager.LoadScene("sampleScene");
    }

    void ResetBotonesColor()
    {
        SetButtonColor(btnProfe0, COL_NORMAL);
        SetButtonColor(btnProfe1, COL_NORMAL);
        SetButtonColor(btnProfe2, COL_NORMAL);
    }

    void SetButtonColor(Button b, Color c)
    {
        if (b == null) return;
        ColorBlock cb = b.colors;
        cb.normalColor = c;
        b.colors = cb;
    }

    Button GetBoton(int i) => i switch { 0 => btnProfe0, 1 => btnProfe1, _ => btnProfe2 };
}