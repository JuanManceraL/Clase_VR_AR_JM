using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Material materialObjeto;

    public void CambiarColorBoton()
    {
        float r = Random.Range(0.0f,1.0f);
        float g = Random.Range(0.0f,1.0f);
        float b = Random.Range(0.0f,1.0f);
        materialObjeto.color = new Color(r,g,b);
    }
}
