using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardTableSystem : MonoBehaviour
{
    //玩家手牌信息
    List<Vector2> handCardPos;
    int currentHandCardNum=0;
    CardController[] handCards;

    //公共牌信息
    List<Vector2> publicCardPos;
    int currentPublicCardNum=0;
    CardController[] publicCards;

    //牌桌背景
    Sprite tableMateria;

    //其它
    public GameObject CardPerfeb;
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    int levelID = 1;//通过ID读取关卡数据

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

        //根据字典数据抽牌到公共区和手牌
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
                        card.isBlock = true;//后续加入图标之类的

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
    /// 读取当前关卡数据
    /// </summary>
    /// <param name="levelID"></param>
    private void LoadData(int levelID)
    {
        //设置牌桌
        LevelBean data = DataReader.Instance.levelDic[levelID];
        tableMateria = Resources.Load<Sprite>(data.CardTable);
        GetComponent<Image>().sprite = tableMateria;
        //设置每个卡牌插槽的坐标
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
    /// 从牌库中添加卡牌到手牌或公共区域
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
                Debug.Log("手牌满了");
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
                Debug.Log("公共区域满了");
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
    /// 从当前区域移动到另一区域
    /// </summary>
    /// <param name="card"></param>
    public void MoveCardToAnotherArea(CardController card)
    {
        //从手牌中移动到公共区域
        if (SearchHandCard(card.data.ID))
        {
            if (card.isBlock)
            {
                Debug.Log("卡牌被锁定");
            }
            else
            {
                //将当前坐标更新为公共牌的空槽位
                if (currentPublicCardNum < publicCards.Length)
                {
                    int index = SearchEmptySlot(false);
                    publicCards[index] = card;
                    card.transform.position = publicCardPos[index];
                    currentHandCardNum--;
                }
                else
                {
                    Debug.Log("无法超出公共区域上限");
                }
            }

        }
        //从公共区域移动到手牌中
        else
        {
            if (card.isBlock)
            {
                Debug.Log("卡牌被锁定");
            }
            else
            {
                //将当前坐标更新为手牌的空槽位
                if (currentHandCardNum < handCards.Length)
                {
                    int index = SearchEmptySlot(true);
                    handCards[index] = card;
                    card.transform.position = handCardPos[index];
                    currentPublicCardNum--;
                }
                else
                {
                    Debug.Log("无法超出手牌上限");
                }
            }
        }
    }

    /// <summary>
    /// 判断手牌中是否有这张牌
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
    /// 获取空卡槽的索引号
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
    /// 卡牌控制器：通过鼠标左键控制
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
                    //揭露卡牌
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
