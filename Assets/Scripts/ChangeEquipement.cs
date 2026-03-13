using UnityEngine;

public class ChangeEquipement : MonoBehaviour
{
    [SerializeField] GameObject[] equipament;
    int lastIndex;

    private void Start()
    {
        lastIndex = equipament.Length;
    }

    public void CambiarEquipBoton()
    {
        foreach (GameObject equip in equipament)
        {
            equip.SetActive(false);
        }

        int index = Random.Range(0,equipament.Length);

        while (index == lastIndex)
        {
            index = Random.Range(0, equipament.Length);
        }

        lastIndex = index;

        equipament[index].SetActive(true);
    }
}
