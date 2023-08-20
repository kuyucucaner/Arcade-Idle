using DG.Tweening;
using System.Collections;
using UnityEngine;

public class workDesk : MonoBehaviour
{
    public Animator femaleAnimator;
    [SerializeField] private Transform dollarPlace;
    [SerializeField] private GameObject dollar;
    private float YAxis;
    private IEnumerator makeMoneyIE;

    private void Start()
    {
        makeMoneyIE = MakeMoney();
    }

    public void Work()
    {
        femaleAnimator.SetBool("work", true);
        StartCoroutine(MakeMoney());
        InvokeRepeating("SubmitPapers", 2f, 1f);
    }

    private IEnumerator MakeMoney()
    {
        var Counter = 0;
        var DollarPlaceIndex = 0;

        yield return new WaitForSecondsRealtime(2);

        while (Counter < transform.childCount)
        {
            GameObject newDollar = Instantiate(dollar, new Vector3(dollarPlace.GetChild(DollarPlaceIndex).position.x, YAxis, dollarPlace.GetChild(DollarPlaceIndex).position.z), dollarPlace.GetChild(DollarPlaceIndex).rotation);

            newDollar.transform.DOScale(new Vector3(0.4f, 0.4f, 0.8f), 0.5f).SetEase(Ease.OutElastic);

            if (DollarPlaceIndex < dollarPlace.childCount - 1)
            {
                DollarPlaceIndex++;
            }
            else
            {
                DollarPlaceIndex = 0;
            }
            YAxis = 0f;
            yield return new WaitForSecondsRealtime(3f);
        }
    }

    private void SubmitPapers()
    {
        if (transform.childCount > 0)
        {
            Transform lastChild = transform.GetChild(transform.childCount - 1);
            if (lastChild != null)
            {
                Destroy(lastChild.gameObject, 1f);
            }
        }
        else
        {
            femaleAnimator.SetBool("work", false);

            Transform desk = transform.parent;
            if (desk != null && desk.childCount > 0)
            {
                desk.GetChild(desk.childCount - 1).GetComponent<Renderer>().enabled = true;
            }

            StopCoroutine(makeMoneyIE);

            YAxis = 0f;
        }
    }
}
