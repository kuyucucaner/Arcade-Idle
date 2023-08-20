using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    private Vector3 direction;
    private Camera Cam;
    [SerializeField] private float playerSpeed = 10;
    private Animator playerAnimation;
    [SerializeField] private List<Transform> papers = new List<Transform>();
    [SerializeField] private Transform paperPlace;
    private float YAxis, delay;
    public TextMeshProUGUI MoneyCounter; 
    public static PlayerManager playerManagerInstance;


    void Start()
    {
        Cam = Camera.main;
        playerAnimation = GetComponent<Animator>();
        playerManagerInstance = this;
        papers.Add(paperPlace);
        PlayerPrefs.SetInt("dollar", 0);
    }
     void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up , transform.position);
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray,out var distance))
            {
                direction = ray.GetPoint(distance);
            }
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(direction.x,0f,direction.z),playerSpeed * Time.deltaTime);
               

            var offset = direction - transform.position;

            if(offset.magnitude > 1f)
            { 
            transform.LookAt(direction);
            }
        }
        if(Input.GetMouseButtonDown(0))
        {
           
            if (papers.Count > 1)
            {
                playerAnimation.SetBool("carry", false);
                playerAnimation.SetBool("RunWithPapers", true);
            }
            else
            {
                playerAnimation.SetBool("run", true);
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            playerAnimation.SetBool("run", false);

            if (papers.Count > 1)
            {
                playerAnimation.SetBool("carry", true);
                playerAnimation.SetBool("RunWithPapers", false);
            }
        }

        if (papers.Count > 1)
        {
            var firstPaper = papers[0].position; // Get the position of the first paper

            for (int i = 1; i < papers.Count; i++)
            {
                var secondPaper = papers[i];

                var offset = new Vector3(0f, i * 0.05f, 0f); // Apply a vertical offset

                secondPaper.position = firstPaper + offset;
            }
        }


        if (Physics.Raycast(transform.position,transform.forward , out var hit , 1f))
        {
            if (hit.collider.CompareTag("table") && papers.Count < 21)
            { 
                 if(hit.collider.transform.childCount > 2) 
                {
                var paper = hit.collider.transform.GetChild(1);
                paper.rotation = Quaternion.Euler(paper.rotation.x,UnityEngine.Random.Range(0f,180f),paper.rotation.z);
                papers.Add(paper);
                paper.parent = null;

                if(hit.collider.transform.parent.GetComponent<Printer>().CountPapers > 1)
                    {
                        hit.collider.transform.parent.GetComponent<Printer>().CountPapers--;
                    }
                if(hit.collider.transform.parent.GetComponent<Printer>().YAxis > 0f)
                    {
                        hit.collider.transform.parent.GetComponent <Printer>().YAxis-= 0.1f;
                    }
                    playerAnimation.SetBool("carry", true);
                    playerAnimation.SetBool("run", false);
                }
            }
            if (hit.collider.CompareTag("pp") && papers.Count > 1)
            {
                var WorkDesk = hit.collider.transform;

                if (WorkDesk.childCount > 0)
                {
                    YAxis = WorkDesk.GetChild(WorkDesk.childCount - 1).position.y;
                }
                else
                {
                    YAxis = WorkDesk.position.y;
                }

                for (var index = papers.Count - 1; index >= 1; index--)
                {
                    papers[index].DOJump(new Vector3(WorkDesk.position.x, YAxis, WorkDesk.position.z), 2f, 1, 0.1f)
                        .SetDelay(delay).SetEase(Ease.Flash);

                    papers.ElementAt(index).parent = WorkDesk;
                    papers.RemoveAt(index);

                    YAxis += 0.1f;
                    delay += 0.2f;
                }

                WorkDesk.parent.GetChild(WorkDesk.parent.childCount - 1).GetComponent<Renderer>().enabled = false; 

                if(papers.Count <= 1)
                {
                    playerAnimation.SetBool("carry", false);
                    playerAnimation.SetBool("RunWithPapers", false);
                    playerAnimation.SetBool("idle", true);
                }
            }
        }
 

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("pp"))
        {
            other.GetComponent<workDesk>().Work();
        }
        else if (other.CompareTag("dollar"))
        {
            // Store the reference to the GameObject before destroying it
            GameObject dollarObject = other.gameObject;

            // Update PlayerPrefs and UI text
            PlayerPrefs.SetInt("dollar", PlayerPrefs.GetInt("dollar") + 5);
            MoneyCounter.text = "$" + PlayerPrefs.GetInt("dollar").ToString("N0");

            // Destroy the object
            Destroy(dollarObject);

            // Your Tween animation code here
            dollarObject.transform.DOScale(new Vector3(0.4f, 0.4f, 0.8f), 0.5f).SetEase(Ease.OutElastic);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("pp"))
        {
            //playerAnimation.SetBool("carry", false);
            playerAnimation.SetBool("RunWithPapers", false);
            playerAnimation.SetBool("idle", false);
            playerAnimation.SetBool("run", true);
            delay = 0f;
        }
        if (other.CompareTag("table"))
        {
            if (papers.Count > 1)
            {
                playerAnimation.SetBool("carry", false);
                playerAnimation.SetBool("RunWithPapers", true);
            }
            else
            {
                playerAnimation.SetBool("run", true);

            }
        }
    }
}
