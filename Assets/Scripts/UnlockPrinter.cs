using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

public class UnlockPrinter : MonoBehaviour
{
    [SerializeField] private GameObject unlockProgressObject;
    [SerializeField] private GameObject newPrinter;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI dollarAmount;
    [SerializeField] private int printerPrice, printerRemainPrice;
    [SerializeField] private float ProgressValue;
    public NavMeshSurface buildNavMesh;

    void Start()
    {
        UpdateDollarAmountText();
        printerRemainPrice = printerPrice;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerPrefs.GetInt("dollar") > 0)
        {
            ProgressValue = Mathf.Abs(1f - CalculateMoney() / printerPrice);

            if (PlayerPrefs.GetInt("dollar") >= printerRemainPrice)
            {
                PlayerPrefs.SetInt("dollar", PlayerPrefs.GetInt("dollar") - printerRemainPrice);
                printerRemainPrice = 0;
            }
            else
            {
                printerRemainPrice -= PlayerPrefs.GetInt("dollar");
                PlayerPrefs.SetInt("dollar", 0);
            }

            progressBar.fillAmount = ProgressValue;
            PlayerManager.playerManagerInstance.MoneyCounter.text = PlayerPrefs.GetInt("dollar").ToString("N0");

            if (printerRemainPrice > 0)
            {
                UpdateDollarAmountText();
            }
            else
            {
                dollarAmount.text = "";
                GameObject printer = Instantiate(newPrinter, new Vector3(transform.position.x, 2f, 23.33f),
                    Quaternion.Euler(-90f, 0f, -90f));

                printer.transform.DOScale(new Vector3(1.93f, 2.08f, 2.02f), 2f).SetEase(Ease.OutElastic);
                printer.transform.DOScale(new Vector3(1.93f, 2.08f, 2.02f), 2f).SetDelay(1.1f).SetEase(Ease.OutElastic);

                unlockProgressObject.SetActive(false);
                buildNavMesh.BuildNavMesh();
            }
        }
    }

    private float CalculateMoney()
    {
        return printerRemainPrice - PlayerPrefs.GetInt("dollar");
    }

    private void UpdateDollarAmountText()
    {
        dollarAmount.text = $"New Printer {printerRemainPrice:N0}";
    }
}
