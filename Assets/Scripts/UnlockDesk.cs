using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnlockDesk : MonoBehaviour
{
    [SerializeField] private GameObject unlockProgressObject;
    [SerializeField] private GameObject newDesk;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI dollarAmount;
    [SerializeField] private int deskPrice, deskRemainPrice;
    [SerializeField] private float ProgressValue;
    public NavMeshSurface buildNavMesh;

    void Start()
    {
        UpdateDollarAmountText();
        deskRemainPrice = deskPrice;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerPrefs.GetInt("dollar") > 0)
        {
            ProgressValue = Mathf.Abs(1f - CalculateMoney() / deskPrice);

            if (PlayerPrefs.GetInt("dollar") >= deskRemainPrice)
            {
                PlayerPrefs.SetInt("dollar", PlayerPrefs.GetInt("dollar") - deskRemainPrice);
                deskRemainPrice = 0;
            }
            else
            {
                deskRemainPrice -= PlayerPrefs.GetInt("dollar");
                PlayerPrefs.SetInt("dollar", 0);
            }

            progressBar.fillAmount = ProgressValue;
            PlayerManager.playerManagerInstance.MoneyCounter.text = PlayerPrefs.GetInt("dollar").ToString("N0");

            if (deskRemainPrice > 0)
            {
                UpdateDollarAmountText();
            }
            else
            {
                dollarAmount.text = "";
                GameObject desk = Instantiate(newDesk, new Vector3(transform.position.x, 3.3f, transform.position.z),
                    Quaternion.Euler(0f, -90f, 0f));

                desk.transform.DOScale(1f, 1f).SetEase(Ease.OutElastic);
                desk.transform.DOScale(1f, 1f).SetDelay(1.1f).SetEase(Ease.OutElastic);

                unlockProgressObject.SetActive(false);
                buildNavMesh.BuildNavMesh();
            }
        }
    }

    private float CalculateMoney()
    {
        return deskRemainPrice - PlayerPrefs.GetInt("dollar");
    }

    private void UpdateDollarAmountText()
    {
        dollarAmount.text = $"New Desk {deskRemainPrice:N0}";
    }
}
