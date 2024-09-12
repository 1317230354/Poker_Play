using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardTableSystem : MonoBehaviour
{
    //���������Ϣ
    List<Vector2> handCardPos;
    int currentHandCardNum=0;
    CardController[] handCards;

    //��������Ϣ
    List<Vector2> publicCardPos;
    int currentPublicCardNum=0;
    CardController[] publicCards;

    //��������
    Sprite tableMateria;

    //����
    public GameObject CardPerfeb;
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    int levelID = 1;//ͨ��ID��ȡ�ؿ�����

    private void Awake()
    {
        handCardPos = new List<Vector2>();
        publicCardPos = new List<Vector2>();

        LoadData(levelID);
    }
    private void Start()
    {
        raycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        //�����ֵ����ݳ��Ƶ�������������
        LoadDefaultCard();

        void LoadDefaultCard()
        {
            LevelBean levelData = DataReader.Instance.levelDic[levelID];
            for (int i = 0; i < levelData.CardOwn.Count; i++)
            {
                if (levelData.CardOwn[i] != 0)
                {
                    CardController card = AddCard(levelData.CardOwn[i]);
                    if (levelData.LockCard[i] == 1)
                        card.isBlock = true;//��������ͼ��֮���

                }
            }
            for (int i = 0; i < levelData.CardPublic.Count; i++)
            {
                if (levelData.CardPublic[i] != 0)
                {
                    CardController card = AddCard(levelData.CardPublic[i]);
                }
            }
        }
    }
    private void Update()
    {
        ControlCard();
    }
    /// <summary>
    /// ��ȡ��ǰ�ؿ�����
    /// </summary>
    /// <param name="levelID"></param>
    private void LoadData(int levelID)
    {
        //��������
        LevelBean data = DataReader.Instance.levelDic[levelID];
        tableMateria = Resources.Load<Sprite>(data.CardTable);
        GetComponent<Image>().sprite = tableMateria;
        //����ÿ�����Ʋ�۵�����
        int maxHandCardNum = data.OwnCardPosX.Count;
        int maxPublicCardNum = data.PublicCardPosX.Count;
        for (int i=0;i<maxHandCardNum;i++)
        {
            handCardPos.Add(new Vector2(data.OwnCardPosX[i], data.OwnCardPosY[i]));
        }
        for(int i=0;i<maxPublicCardNum;i++)
        {
            publicCardPos.Add(new Vector2(data.PublicCardPosX[i], data.PublicCardPosY[i]));
        }
        handCards=new CardController[maxHandCardNum];
        publicCards = new CardController[maxPublicCardNum];
    }

    /// <summary>
    /// ���ƿ�����ӿ��Ƶ����ƻ򹫹�����
    /// </summary>
    /// <param name="cardID"></param>
    /// <param name="Hand"></param>
    public CardController AddCard(int cardID,bool Hand=true)
    {
        Dictionary<int,CardBean>cardDic= DataReader.Instance.cardDic;
        if(Hand)
        {
            if(currentHandCardNum<handCards.Length)
            {
                
                for(int i=0;i<handCards.Length;i++)
                {
                    if (handCards[i]==null)
                    {
                        CardController card = CreateCard();
                        card.OpenCard();
                        card.transform.position = handCardPos[i];
                        handCards[i]=card;
                        currentHandCardNum++;
                        return card;
                    }
                }
            }
            else
            {
                Debug.Log("��������");
            }
            return null;
        }
        else
        {
            if(currentPublicCardNum<publicCards.Length)
            {
                for (int i = 0; i < publicCards.Length; i++)
                {
                    if (publicCards[i] == null)
                    {
                        CardController card = CreateCard();
                        card.CoverCard();
                        card.transform.position = publicCardPos[i];
                        publicCards[i] = card;
                        currentPublicCardNum++;
                        return card;
                    }
                }
                
            }
            else
            {
                Debug.Log("������������");
            }
            return null;
        }
        CardController CreateCard()
        {
            CardController card = Instantiate(CardPerfeb,transform,false).GetComponent<CardController>();
            card.data = cardDic[cardID];
            return card;
        }
    }
    
    /// <summary>
    /// �ӵ�ǰ�����ƶ�����һ����
    /// </summary>
    /// <param name="card"></param>
    public void MoveCardToAnotherArea(CardController card)
    {
        //���������ƶ�����������
        if (SearchHandCard(card.data.ID))
        {
            if (card.isBlock)
            {
                Debug.Log("���Ʊ�����");
            }
            else
            {
                //����ǰ�������Ϊ�����ƵĿղ�λ
                if (currentPublicCardNum < publicCards.Length)
                {
                    int index = SearchEmptySlot(false);
                    publicCards[index] = card;
                    card.transform.position = publicCardPos[index];
                    currentHandCardNum--;
                }
                else
                {
                    Debug.Log("�޷�����������������");
                }
            }

        }
        //�ӹ��������ƶ���������
        else
        {
            if (card.isBlock)
            {
                Debug.Log("���Ʊ�����");
            }
            else
            {
                //����ǰ�������Ϊ���ƵĿղ�λ
                if (currentHandCardNum < handCards.Length)
                {
                    int index = SearchEmptySlot(true);
                    handCards[index] = card;
                    card.transform.position = handCardPos[index];
                    currentPublicCardNum--;
                }
                else
                {
                    Debug.Log("�޷�������������");
                }
            }
        }
    }

    /// <summary>
    /// �ж��������Ƿ���������
    /// </summary>
    /// <param name="cardID"></param>
    /// <returns></returns>
    bool SearchHandCard(int cardID)
    {
        for(int i=0;i<handCards.Length;i++)
        {
            if (handCards[i].data.ID == cardID)
                return true;
        }
        return false;
    }
    /// <summary>
    /// ��ȡ�տ��۵�������
    /// </summary>
    /// <param name="isHand"></param>
    /// <returns></returns>
    int SearchEmptySlot(bool isHand=true)
    {
        if(isHand)
        {
            for(int i=0;i<handCards.Length;i++)
            {
                if (handCards[i]==null)
                {
                    return i;
                }
            }
            return -1;
        }
        else
        {
            for (int i = 0; i < publicCards.Length; i++)
            {
                if (publicCards[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
    }

    /// <summary>
    /// ���ƿ�������ͨ������������
    /// </summary>
    void ControlCard()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> result = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, result);
            foreach (RaycastResult r in result)
            {
                if (r.gameObject.CompareTag("closeCard"))
                {
                    //��¶����
                    r.gameObject.GetComponent<CardController>().OpenCard();
                   
                }
                else if (r.gameObject.CompareTag("openCard"))
                {
                    CardController card = r.gameObject.GetComponent<CardController>();
                    MoveCardToAnotherArea(card);
                }
            }
        }
    }

    
}
