using System.Collections;
using UnityEngine;
using Vuforia;

public class MoveTarget : MonoBehaviour
{
    public GameObject model;
    public ObserverBehaviour[] ImageTargets;
    public int currentTarget;
    public float speed = 1.0f;
    private bool isMoving = false;

    public Animator anim;
    public GameObject[] equipament;

    public void PutAnimation(int targetInt)
    {
        foreach (GameObject equip in equipament)
        {
            equip.SetActive(false);
        }

        switch (targetInt)
        {
            case 0:
                anim.SetBool("Idle", true);
                equipament[4].SetActive(true);
                break;
            case 1:
                anim.SetBool("Espada", true);
                equipament[0].SetActive(true);
                equipament[1].SetActive(true);
                break;
            case 2:
                anim.SetBool("Arco", true);
                equipament[2].SetActive(true);
                equipament[3].SetActive(true);
                break;
            case 3:
                anim.SetBool("Idle", true);
                equipament[5].SetActive(true);
                break;
            default:
                break;
        }
    }

    public void moveToNextMarket()
    {
        if (!isMoving)
        {
            anim.SetBool("Espada", false);
            anim.SetBool("Arco", false);
            anim.SetBool("Idle", false);
            StartCoroutine(MoveModel());
        }
    }

    private IEnumerator MoveModel()
    {
        isMoving = true;
        anim.SetBool("Caminar", true);
        ObserverBehaviour target = GetNextDetectedTarget();
        if (target == null)
        {
            isMoving = false;
            yield break;
        }

        Vector3 startPosition = model.transform.position;
        Vector3 endPosition = target.transform.position;

        model.transform.LookAt(new Vector3(endPosition.x, model.transform.position.y, endPosition.z));
        //model.transform.rotation = Quaternion.Euler(0, , 0);
        //model.transform.LookAt(new Vector3(model.transform.position.x, endPosition.y, model.transform.position.z));
        //Vector3 posicionObjetivoIgualada = new Vector3(endPosition.x, model.transform.position.y, endPosition.z);
        //transform.LookAt(posicionObjetivoIgualada);
        float journey = 0;

        while (journey <= 1f)
        {
            journey += Time.deltaTime * speed;
            model.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
            yield return null;
        }

        currentTarget = (currentTarget + 1) % ImageTargets.Length;
        isMoving = false;
        anim.SetBool("Caminar", false);
    }

    private ObserverBehaviour GetNextDetectedTarget()
    {
        int nextTarget = (currentTarget + 1) % ImageTargets.Length;
        if (ImageTargets[nextTarget] != null && (ImageTargets[nextTarget].TargetStatus.Status == Status.TRACKED || ImageTargets[nextTarget].TargetStatus.Status == Status.EXTENDED_TRACKED))
        {
            return ImageTargets[nextTarget];
        }
        foreach (ObserverBehaviour target in ImageTargets)
        {
            if (target != null && (target.TargetStatus.Status == Status.TRACKED || target.TargetStatus.Status == Status.EXTENDED_TRACKED))
            {
                return target;
            }
        }
        return null;
    }
}
