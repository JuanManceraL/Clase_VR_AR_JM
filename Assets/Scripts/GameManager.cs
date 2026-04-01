using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Vuforia;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Escena[] escenas;
    public Escena escenaAct;
    public int actDialogo;
    public int progreso;
    public TextMeshProUGUI textDialogo;
    private string[] textoActual;

    public int ubiPlayer;
    public bool[] detectados;
    public int cantaDet;

    public GameObject botonCaminar;
    public GameObject botonSiguiente;

    bool gameStarted;

    [SerializeField] GameObject infoBuscarZelda;
    [SerializeField] GameObject infoFinDelJuego;
    [SerializeField] GameObject panelFondo;
    [SerializeField] GameObject panelCambioColor;

    [Header("MoveTarget")]
    public GameObject model;
    public ObserverBehaviour[] ImageTargets;
    public ObserverBehaviour lastImageTargets;
    public int currentTarget;
    public float speed = 1.0f;
    private bool isMoving = false;

    [Header("Utileria de escenas")]
    public Animator animZelda;
    public Animator animLink;
    public GameObject[] equipament;
    public GameObject[] escenarios;
    public GameObject enemigos;

    void Start()
    {
        gameStarted = false;
        for (int i = 0; i < detectados.Length; i++)
        {
            detectados[i] = false;
        }
        cantaDet += ImageTargets.Length;
        progreso = 0;
        botonCaminar.SetActive(true);
        
        botonSiguiente.SetActive(false);
        RevolverEscenas();
        UbicarElementosCartas();

        foreach (GameObject @object in equipament)
        {
            @object.SetActive(false);
        }
    }

    void RevolverEscenas()
    {
        int cantScenes = escenas.Length;
        for (int i = cantScenes-1; i > 0; i--)
        {
            int j = Random.Range(0, i+1);
            Escena temp = escenas[i];
            escenas[i] = escenas[j];
            escenas[j] = temp;
            //Debug.Log("CAmbiando:  " + i + "  por   " + j);
        }
    }

    void UbicarElementosCartas()
    {
        for (int i = 0; i < escenas.Length; i++)
        {
            Debug.Log("Escenario " + i + " será hijo de " + escenas[i].ubiProceso);
            //escenarios[i].transform.SetParent(ImageTargets[escenas[i].ubiProceso].transform);
            int escenaProg = escenas[i].ubiProceso;
            escenarios[escenaProg].transform.SetParent(ImageTargets[i].transform);
            
            escenarios[escenaProg].transform.localPosition = Vector3.zero;
            
            if (escenas[i].ubiProceso == 0)
            {
                ubiPlayer = i;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //model.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void TarDetected(int detected)
    {
        cantaDet++;
        detectados[detected] = true;
        if (!gameStarted && escenas[detected].ubiProceso == 0)
        {
            gameStarted = true;
            animZelda.SetBool("Idle", true);
            escenaAct = escenas[detected];
            SiguienteEscenario();
        }
        if (detected == ubiPlayer)
        {
            infoBuscarZelda.SetActive(false);
        }
    }

    public void TarLosted(int detected)
    {
        cantaDet--;
        detectados[detected] = false;
        if (detected == ubiPlayer && gameStarted)
        {
            infoBuscarZelda.SetActive(true);
        }
    }

    public void SalirAlMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void moveToNextMarket()
    {
        if (!isMoving && detectados[ubiPlayer] == true && cantaDet == 2)
        {
            //animZelda.SetBool("Espada", false);
            //animZelda.SetBool("Arco", false);
            animZelda.SetBool("Idle", false);
            animZelda.SetBool("Caminar", true);
            StartCoroutine(MoveModel());
        }
        else
        {
            Debug.Log("NOMEMUEVOO_----------");
        }
    }

    private IEnumerator MoveModel()
    {
        isMoving = true;
        if (progreso >= 2)
        {
            animLink.SetBool("Caminar", true);
            GameObject GL = animLink.gameObject;
            GL.transform.localPosition = new Vector3(-0.2f, 0, 0);
            GL.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        GameObject GZ = animZelda.gameObject;
        GZ.transform.localPosition = new Vector3(0.2f, 0, 0);
        GZ.transform.localRotation = Quaternion.Euler(0, 0, 0);

        int intTarget = 0;
        for (int i = 0; i < detectados.Length; i++)
        {
            if (detectados[i] == true && i != ubiPlayer)
            {
                intTarget = i;
            }
        }
        ObserverBehaviour target = ImageTargets[intTarget];
        escenarios[escenas[ubiPlayer].ubiProceso].SetActive(false);
        ubiPlayer = intTarget;
        if (target == null)
        {
            isMoving = false;
            yield break;
        }

        model.transform.parent = target.transform;
        animZelda.SetBool("Caminar", true);

        Vector3 startPosition = model.transform.position;
        Vector3 endPosition = target.transform.position;

        bool travelCompleted = false;

        while (!travelCompleted)
        {
            endPosition = target.transform.position;

            model.transform.LookAt(endPosition);
            model.transform.localRotation = Quaternion.Euler(0, model.transform.localEulerAngles.y, 0);

            float step = speed * Time.deltaTime;
            model.transform.position = Vector3.MoveTowards(model.transform.position, endPosition, step);
            float distancia = Vector3.Distance(model.transform.position, endPosition);
            if (distancia <= 0.1f)
            {
                travelCompleted = true;
            }
            yield return null;
        }

        isMoving = false;
        animZelda.SetBool("Caminar", false);
        animZelda.SetBool("Idle", true);

        if (progreso >= 2)
        {
            animLink.SetBool("Caminar", false);
        }

        model.transform.localRotation = Quaternion.Euler(0, 0, 0);
        escenaAct = escenas[ubiPlayer];
        escenarios[escenas[ubiPlayer].ubiProceso].SetActive(true); 
        SiguienteEscenario();
    }

    private void SiguienteEscenario()
    {
        if (escenaAct == null)
        {
            return;
        }
        if (escenaAct.ubiProceso == progreso)
        {
            IniciarDialogo(escenaAct.dialogoAct);
            progreso++;
        }
        else if (progreso < escenaAct.ubiProceso)
        {
            IniciarDialogo(escenaAct.dialogoPrevio);
        }
        else
        {
            IniciarDialogo(escenaAct.dialogoPost);

        }
    }

    void IniciarDialogo(string[] texto)
    {
        textoActual = texto;
        actDialogo = 0;
        SiguienteDialogo();

        botonCaminar.SetActive(false);
        botonSiguiente.SetActive(true);
        //Poner en verdadero booleano / aparecer botones
        model.transform.localPosition = new Vector3(0f, 0, 0f);
        model.transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (escenaAct.ubiProceso != progreso)
        {
            return;
        }
        GameObject GZ = animZelda.gameObject;
        switch (escenaAct.ubiProceso)
        {
            /*ACCESORIOS
             * Libro        0
             * tablet       1
             * arco         2
             * flecha       3
             * platillo     4
             * espada       5
             * escudo       6
             */
            case 0: // Inicio de juego
                //Debug.Log("Iniciando primer dialogo");
                equipament[0].SetActive(true);
                model.transform.localPosition = new Vector3(-0.15f, 0, 0.23f);
                model.transform.localRotation = Quaternion.Euler(0, 100, 0);
                break;
            case 1: //LLega con link
                model.transform.localPosition = new Vector3(-0.1f, 0, 0.08f);
                model.transform.localRotation = Quaternion.Euler(0, 150, 0);
                break;
            case 2: //Link ataca
                model.transform.localPosition = new Vector3(0f, 0, -0.44f);
                model.transform.localRotation = Quaternion.Euler(0, 50, 0);
                animLink.SetTrigger("Atacar");
                equipament[5].SetActive(true);
                equipament[6].SetActive(true);
                break;
            case 3: //cazan animales
                model.transform.localPosition = new Vector3(0f, 0, 0f);
                model.transform.localRotation = Quaternion.Euler(0, 0, 0);
                GZ.transform.localPosition = new Vector3(0.2f, 0, 0.6f);
                animZelda.SetTrigger("Disp");
                break;
            case 4: //Preparan comida
                model.transform.localPosition = new Vector3(0f, 0, -0.11f);
                model.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case 5: //Cambio de ropa
                model.transform.localPosition = new Vector3(0f, 0, -0.15f);
                model.transform.localRotation = Quaternion.Euler(0, 180, 0);
                equipament[4].SetActive(false);
                break;
            case 6: // FIesta final
                model.transform.localPosition = new Vector3(0f, 0, 0f);
                model.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            default:
                break;
        }
    }
    public void SiguienteDialogo()
    {
        if (actDialogo >= textoActual.Length)
        {
            TerminarDialogo();
            return;
        }
        string text = textoActual[actDialogo];
        textDialogo.SetText(text);
        actDialogo++;
        if (actDialogo >= textoActual.Length)
        {
            TerminarDialogo();
            return;
        }
    }

    void TerminarDialogo()
    {
        botonCaminar.SetActive(true);
        botonSiguiente.SetActive(false);
        //Debug.Log("-----------------FINAL DEL DIALOGO");
        if (escenaAct.ubiProceso == progreso-1)
        {
            GameObject GZ = animZelda.gameObject;
            switch (progreso-1)
            /*ACCESORIOS
            * Libro        0
            * tablet       1
            * arco         2
            * flecha       3
            * platillo     4
            * espada       5
            * escudo       6
            */
            {
                case 0: //Inicio
                    equipament[0].SetActive(false);
                    equipament[1].SetActive(true);
                    animZelda.SetBool("Caminar", true);
                    animZelda.SetBool("Caminar", false);
                    
                    break;
                case 1: //Llega con link
                    GameObject GL = animLink.gameObject;
                    GL.transform.SetParent(model.transform);
                    GL.transform.localPosition = new Vector3(-0.2f, 0, 0);
                    GL.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    GZ.transform.localPosition = new Vector3(0.2f, 0, 0);
                    GZ.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    //Cambiar posiciones
                    break;
                case 2: //Link va a pelear
                    animLink.SetTrigger("FinAnim");
                    equipament[5].SetActive(false);
                    equipament[6].SetActive(false);
                    enemigos.SetActive(false);
                    equipament[1].SetActive(false);
                    equipament[2].SetActive(true);
                    equipament[3].SetActive(true);
                    break;
                case 3: //cazan animales
                    equipament[2].SetActive(false);
                    equipament[3].SetActive(false);
                    equipament[1].SetActive(true);
                    animZelda.SetTrigger("FinAnim");
                    GZ.transform.localPosition = new Vector3(0.2f, 0, 0.0f);
                    break;
                case 4: //Preparan comida
                    equipament[1].SetActive(false);
                    equipament[4].SetActive(true);
                    break;
                case 5: //Cambio de ropa
                    equipament[4].SetActive(true);
                    //Activar cambiador de ropa.
                    panelCambioColor.SetActive(true);
                    panelFondo.SetActive(false);
                    break;
                case 6: // FIesta final
                    botonCaminar.SetActive(false);
                    panelFondo.SetActive(false);
                    infoFinDelJuego.SetActive(true);
                    animLink.SetTrigger("Dancing");
                    animZelda.SetTrigger("Dancing");
                    break;
                default:
                    break;
            }
            //progreso++;
        }
    }



    private ObserverBehaviour GetNextDetectedTarget()
    {
        /*
        int nextTarget = (currentTarget + 1) % ImageTargets.Length;
        if (ImageTargets[nextTarget] != null && (ImageTargets[nextTarget].TargetStatus.Status == Status.TRACKED || ImageTargets[nextTarget].TargetStatus.Status == Status.EXTENDED_TRACKED))
        {
            return ImageTargets[nextTarget];
        }*/
        foreach (ObserverBehaviour target in ImageTargets)
        {
            if (target != null && (target.TargetStatus.Status == Status.TRACKED || target.TargetStatus.Status == Status.EXTENDED_TRACKED))
            {
                return target;
            }
        }
        return null;
    }

    public void ReiniciarEscena()
    {
        SceneManager.LoadScene("Aventura_Zelda");
    }

    public void SalirApp()
    {
        Application.Quit();
    }
}
