using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public Text leftChipsNum;
    public Text rightChipsNum;
    public Text chooseChipsNum;
    public Slider slider;

    private void Start()
    {
        slider.onValueChanged.AddListener((value) =>
        {
            SetChooseNum(DataBase.Instance.PlayerData.leftChips);
        });
    }
    public void SetView(string leftNum,string rightNum)
    {
        leftChipsNum.text = leftNum;
        rightChipsNum.text = rightNum;
        chooseChipsNum.text = "0";
    }
    public void SetChooseNum(int leftNum)
    {
        int delete = (int)(leftNum * slider.value);
        DataBase.Instance.PlayerData.selectChips = delete;
        chooseChipsNum.text = delete.ToString();
    }

}
