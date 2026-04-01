using UnityEngine;

[CreateAssetMenu(fileName = "Escena", menuName = "Scriptable Objects/Escena")]
public class Escena : ScriptableObject
{
    public int ubiProceso;
    [TextArea(4, 8)]
    public string[] dialogoPrevio;
    [TextArea(4, 8)]
    public string[] dialogoAct;
    [TextArea(4, 8)]
    public string[] dialogoPost;
}
