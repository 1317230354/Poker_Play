using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public Image color1, color2,backGround;
    public Text num1, num2;

    [HideInInspector]
    public CardBean data;

    public bool isBlock = false;//������Ĭ�ϲ�Ϊ����״̬

    public void CoverCard()
    {
        color1.enabled = false;
        color2.enabled = false;
        num1.enabled = false;
        num2.enabled = false;
        GetComponent<Image>().sprite = Resources.Load<Sprite>(data.CardBack);
        //����ǩ�滻Ϊ�ر�״̬
        tag = "closeCard";
    }
    public void OpenCard()
    {
        color1.enabled = true;
        color2.enabled = true;
        num1.enabled = true;
        num2.enabled = true;
        GetComponent<Image>().sprite = Resources.Load<Sprite>(data.CardFront);
        //����ǩ�滻Ϊ����״̬
        tag = "openCard";
    }
}
