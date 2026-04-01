using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Material materialObjeto;
    public Color[] coloresEst;
    public Color colorOriginal;
    public GameObject panelCambioColor;
    public GameObject panelFondo;

    private void Start()
    {
        PonerColorOriginal();
    }

    public void CambiarColorBoton()
    {
        float r = Random.Range(0.0f,1.0f);
        float g = Random.Range(0.0f,1.0f);
        float b = Random.Range(0.0f,1.0f);
        materialObjeto.color = new Color(r,g,b);
    }

    public void ElegirColor(int colorId)
    {
        if (colorId < 0 || colorId >= coloresEst.Length)
            return;

        materialObjeto.color = coloresEst[colorId];
    }

    public void PonerColorOriginal()
    {
        materialObjeto.color = colorOriginal;
    }

    public void SiguienteEtapa()
    {
        panelCambioColor.SetActive(false);
        panelFondo.SetActive(true);
    }
}
