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
    public bool isCover = true;//���Ʋ�Ϊ����״̬
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
                    //��¶����(���ֱ�ӱ�ɫ�����
                    backGround.color = new Color(0, 0, 0.5f);
                    //Ȼ�󽫱�ǩ�滻Ϊ����״̬
                    r.gameObject.tag = "openCard";
                    isCover = false;
                    //�����������ʾ
                    color1.enabled = true;
                    color2.enabled = true;
                    num1.enabled = true;
                    num2.enabled = true;
                }
                else if(r.gameObject.CompareTag("openCard"))
                {
                    //�ж������������,�ҵ�ǰ���겻������ʼ���꣬�ƶ�����ʼλ��
                    if(isOwn)
                    {
                        transform.position = originPos;
                    }
                    //������ӹ��������ƶ��������У��������û����
                    else
                    {
                        //����ǰ�������Ϊ������Ƶ���һ����λ
                    }
                }
            }
        }
    }
}
