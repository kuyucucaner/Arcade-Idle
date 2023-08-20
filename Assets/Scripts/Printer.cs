using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class Printer : MonoBehaviour
{
    [SerializeField] private Transform[] PaperPlace = new Transform[10];
    [SerializeField] private GameObject paper;
    public float PapersDeliveryTime = 0.5f; 
    public float YAxis;
    public int CountPapers;
    // Start is called before the first frame update
    void Start()
    {   
        for(int i = 0; i < PaperPlace.Length; i++)
        {
            PaperPlace[i] = transform.GetChild(0).GetChild(i);
        }
        StartCoroutine(PrintPaper(PapersDeliveryTime));
    }

    public IEnumerator PrintPaper(float Time)
    {
       
        var PP_index = 0;
        while (CountPapers < 10)
        {

            GameObject NewPaper = Instantiate(paper, new Vector3(transform.position.x, -3f, transform.position.z),
                Quaternion.identity, transform.GetChild(1));

            NewPaper.transform.DOJump(new Vector3(PaperPlace[PP_index].position.x, PaperPlace[PP_index].position.y + YAxis,
                PaperPlace[PP_index].position.z), 2f, 1, 0.5f).SetEase(Ease.OutQuad);

           // Debug.Log("Sýçrama Koordinatý: " + PaperPlace[PP_index].position);

            if (PP_index < 9)
            {
                PP_index++;
            }
            else
            {
                PP_index = 0;
                YAxis += 0.1f;
            }

            yield return new WaitForSecondsRealtime(Time);
        }
    }

}
