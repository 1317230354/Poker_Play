using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public Image color1, color2,backGround;
    public Text num1, num2;
    public Vector2 originPos;
    [HideInInspector]
    public bool isCover = true;//手牌不为覆盖状态
    [HideInInspector]
    public bool isOwn=false;
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    private void Start()
    {
        raycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
        if(isCover)
        {
            color1.enabled = false;
            color2.enabled = false;
            num1.enabled = false;
            num2.enabled = false;
        }
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> result = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, result);
            foreach(RaycastResult r in result)
            {
                if(r.gameObject.CompareTag("closeCard"))
                {
                    //揭露卡牌(这边直接变色替代）
                    backGround.color = new Color(0, 0, 0.5f);
                    //然后将标签替换为开启状态
                    r.gameObject.tag = "openCard";
                    isCover = false;
                    //激活卡牌数字显示
                    color1.enabled = true;
                    color2.enabled = true;
                    num1.enabled = true;
                    num2.enabled = true;
                }
                else if(r.gameObject.CompareTag("openCard"))
                {
                    //判断如果在手牌中,且当前坐标不等于起始坐标，移动到起始位置
                    if(isOwn)
                    {
                        transform.position = originPos;
                    }
                    //不在则从公共区域移动到手牌中（如果手牌没满）
                    else
                    {
                        //将当前坐标更新为玩家手牌的下一个槽位
                    }
                }
            }
        }
    }
}
