using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Text num1, num2;
    public Image color1, color2;
    public Image cardIcon;
    [HideInInspector]
    public int ID;
    [HideInInspector]
    public bool isSelected=false;

    public void SetView(string num1,string num2,string color1,string color2)
    {
        this.num1.text = num1;
        this.num2.text = num2;
        this.color1.sprite = Resources.Load<Sprite>(color1);
        this.color2.sprite = Resources.Load<Sprite>(color2);
    }
    public void Revert(bool isRevert,string iconPath)
    {
        color1.gameObject.SetActive(!isRevert);
        color2.gameObject.SetActive(!isRevert);
        num1.gameObject.SetActive(!isRevert);
        num2.gameObject.SetActive(!isRevert);
        cardIcon.sprite = Resources.Load<Sprite>(iconPath);
    }
}
