using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour
{
    public Image icon;
    public new Text name;
    public List<GameObject> miniCards;
    public Text leftNum, poolNum;
    public int uid;
    public void SetView(string name,string chips,string poolChips)
    {
        //icon
        this.name.text = name; 
        leftNum.text = chips;
        poolNum.text = poolChips;
        for(int i = 0; i < miniCards.Count; i++)
        {
            miniCards[i].SetActive(false);
        }

    }
    public void UpdataView(string chips,string poolChips)
    {
        leftNum.text = chips;
        poolNum.text = poolChips;
    }

    public void ChangeColor(bool isRecory=false,bool isDead =false)
    {
        if(isDead)
        {
            GetComponent<Image>().color = new Color(1, 0, 0);
            return;
        }
        if(isRecory==false)
            GetComponent<Image>().color = new Color(0, 1, 0);
        else
            GetComponent<Image>().color = new Color(0.9f, 0.7f, 0.55f);
    }
    public void ShowMiniCard()
    {
        for(int i=0;i<miniCards.Count;i++)
        {
            miniCards[i].SetActive(true);
        }
    }
    public void SetMiniCardView(CardData card,int index)
    {
        Text miniNum = miniCards[index].transform.Find("Num").GetComponent<Text>();
        Image miniColor = miniCards[index].transform.Find("Color").GetComponent<Image>();
        miniNum.text = card.num;
        miniColor.sprite = Resources.Load<Sprite>(card.colorIcon);
    }
}
