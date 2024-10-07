using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CoreSystem : MonoBehaviour
{
    //������
    private static CoreSystem instance;
    public static CoreSystem Instance => instance;
    DataReader dataReader;
    DataBase dataBase;
    TurnSystem turnSystem;
    //����Ԥ����
    public GameObject PlayerPanel;
    public GameObject CharacterA,CharacterB;
    public GameObject CardPerfebs;
    public GameObject GamePanel;
    public Transform CanvasTransform;
    public GameObject SureBtnPerfeb;
    //��������
    [HideInInspector]
    public PlayerView playerView;
    [HideInInspector]
    public List<CharacterView> characterViews;
    List<CardView> cardViews;
    [HideInInspector]
    public GameView gameView;
    //��������
    [HideInInspector]
    public int currentBigTurn=1;
    int maxBigTurn=3;
    bool canBeChoose = false;
    Button sureBtn = null;
    List<PricePoolData> pricePools=new List<PricePoolData>();
    //��������
    int levelID=1;

    private void Awake()
    {
        instance = this;

    }
    private void Start()
    {
        dataReader = DataReader.Instance;
        dataBase = DataBase.Instance;
        turnSystem = TurnSystem.Instance;
        LoadLevel(levelID);
    }

    public void DestroyInstance()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;  // ȷ����̬���ñ����
        }
    }

    void LoadLevel(int id)
    {
        //��ʼ������
        InitScene();

        LevelBean level = GetLevel(id);
        //������Ϸ����
        GameObject panel = Instantiate(GamePanel, CanvasTransform);
        gameView = panel.GetComponent<GameView>();
        gameView.SetView(level.CardTable);
        //�����������
        PlayerData playerData = new PlayerData { leftChips = 1000, poolChips = 0, selectChips = 0 };//��ʱ����
        DataBase.Instance.PlayerData = playerData;
        //����������
        GameObject table = Instantiate(PlayerPanel, panel.transform);
        table.GetComponent<PlayerView>().SetView(playerData.leftChips.ToString(), playerData.poolChips.ToString());
        playerView = table.GetComponent<PlayerView>();
        //����npc��ɫ
        List<GameObject> npcs = GenerateNPC(level, panel);
        for (int i = 0; i < npcs.Count; i++)
        {
            characterViews.Add(npcs[i].GetComponent<CharacterView>());
        }
        //��ʼ��ÿ������
        InitCardTable();
        //����
        LoadAllCard();
        //��ת����
        ReverAllCards();
        //��ʼ�غ�
        turnSystem.SetSeat();//��ɫ����
        turnSystem.SetIndex(0);//������ʼ�غ�λ�ã�0Ϊ��ң�
        turnSystem.StartTurn();//�غϿ�ʼ

    }


    /// <summary>
    /// Test
    /// </summary>
    public void GeneratePricePool()
    {
        const int MINNUM = 1000000000;
        //��ȡ���н�ɫ�ĳ����
        //����ص�˳��Ϊ����ҿ�ʼ ˳ʱ�� ����
        int playerChips = dataBase.PlayerData.poolChips;
        List<int>npcChips = new List<int>();
        List<NPC> NPCs = turnSystem.GetAllNpc();
        for(int i=0;i<NPCs.Count;i++)
        {
            npcChips.Add(NPCs[i].data.poolChips);
        }
        //���ɳ����
        int minNum = MINNUM;
        int j = 0;
        while(minNum>0&&j<100)
        {
            j++;//����
            minNum = Min();
            pricePools.Add(GeneratePool(minNum));
            
        }

        //Debug.Log(j);
        //Debug.Log("�����Num:"+pricePools.Count);

        //��ȡ��С����س�������
        int Min()
        {
            int minChips = MINNUM;
            if(minChips>playerChips&&playerChips!=0)
            {
                minChips = playerChips;
            }
            foreach(var chips in npcChips)
            {
                if(minChips>chips&&chips!=0)
                {
                    minChips = chips;
                }
            }
            if (minChips == MINNUM)
                minChips = 0;
            return minChips;
        }
        //�۳����г���ض�Ӧ���벢����1������
        PricePoolData GeneratePool(int chips)
        {
            PricePoolData pool = new PricePoolData();
            if(playerChips>=chips)
            {
                dataBase.PlayerData.poolChips -= chips;
                playerChips -= chips;
                pool.joiner.Add(0);
                pool.Price += chips;
            }
            for(int i=0;i<npcChips.Count;i++)
            {
                if(npcChips[i]>=chips)
                {
                    NPCs[i].data.poolChips -= chips;
                    npcChips[i] -= chips;
                    pool.joiner.Add(i+1);
                    pool.Price += chips;
                }
            }
            return pool;
        }

    }
    /// <summary>
    /// Test
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public int MatchWeight(List<CardData> cards)
    {
        //����
        cards.Sort((card1,card2) =>card2.weight.CompareTo(card1.weight));

        //Debug.Log("��ĸ����Start");
        //foreach (var card in cards)
        //{

        //    Debug.Log(card.num);
        //}
        //Debug.Log("��ĸ����END");

        //ƥ������
        int weight = 1000;
        foreach (var type in dataReader.targetDic)
        {
            //�ж����ͻ�ɫ
            if(type.Value.ColorType==1)
            {
                bool isSame = true;
                int colorType = cards[0].colorType;
                for(int i=0;i<cards.Count;i++)
                {
                    if(cards[i].colorType!=colorType)
                    {
                        isSame = false;
                        break;
                    }
                    
                }
                if(isSame==false)
                {
                    continue;
                }
            }

            //�ж����͵���
            List<string> condiction = type.Value.Condiction;
            int sameTime = 0;
            int index = -1;
            for(int i=0;i<condiction.Count;i++)
            {
                index++;
                if (condiction[i] =="11")
                {
                    if (cards[index].num == cards[index+1].num)
                    {
                        sameTime += 2;
                        index++;
                    }
                }
                else if(condiction[i]=="0")
                {
                    sameTime++;
                }
                else if (condiction[i]=="-1")
                {
                    //���ƣ�ȡ������+��ǰȨ��
                    sameTime = -1;
                    break;
                }
                else
                {
                    if (cards[index].num == condiction[i])
                    {
                        sameTime++;
                    }
                }
            }
            if(sameTime==cards.Count&&type.Value.Weight<weight)
            {
                weight=type.Value.Weight;
            }
            else if(sameTime==-1&& (type.Value.Weight + cards[cards.Count - 1].weight)<weight)
            {
                weight= (type.Value.Weight + cards[cards.Count - 1].weight);
            }
        }
        return weight;
    }
    public void GivePrice()
    {
        const int MAX = 1000;
        //�ȼ������н�ɫ�����ʹ�С
        List<NPC> npcList = turnSystem.GetAllNpc();
        List<int> seatWeight = new List<int>();
        SetWeight(MAX, npcList, seatWeight);
        for(int i=0; i<seatWeight.Count;i++)
        {
            Debug.Log($"��λ��{i}Ȩ�أ�{seatWeight[i]}");
        }
        //�������н��أ��Ƚ����в����ߵ����ʹ�С��ѡ��ʤ���߲����佱�ؽ���
        for (int i = 0; i < pricePools.Count; i++)
        {
            List<int> seatID = pricePools[i].joiner;//��������λ�ż���
            //ѡ��ʤ��������Ȩ��
            int min = MAX;
            for (int j=0; j<seatID.Count;j++)
            {
                int seatIndex = seatID[j];
                if (seatWeight[seatIndex]< min)
                {
                    min = seatWeight[seatIndex];
                }
            }
            //����ȫ������Ҫ��Ĳ�����
            List<int> winnerID = new List<int>();
            for(int j=0;j<seatID.Count;j++)
            {
                int seatIndex = seatID[j];
                if (min == seatWeight[seatIndex])
                {
                    winnerID.Add(seatIndex);
                }
            }
            //����ʤ��������ƽ���������Ӧ��ɫ
            int price = pricePools[i].Price / winnerID.Count;
            for(int j=0;j<winnerID.Count;j++)
            {
                int seatIndex = winnerID[j];

                if (seatIndex ==0)
                {
                    dataBase.PlayerData.leftChips += price;
                    continue;
                }
                npcList[seatIndex-1].data.leftChips+=price;

            }

            //Debug.Log("����" + (i + 1) + ":" + winnerID.Count);
            //for(int j=0;j<winnerID.Count;j++)
            //{
            //    Debug.Log("ʤ�ߣ�" + winnerID[j] + "Ȩ��:" + seatWeight[winnerID[j]]);
                
            //}

        }
        //ˢ�����
        turnSystem.UpdatePanel();
        //Debug.Log("�ַ�����ˢ�����");
        void SetWeight(int MAX, List<NPC> npcList, List<int> seatWeight)
        {
            List<CardData> cards = selectedCard;
            seatWeight.Add(MatchWeight(cards));
            for (int i = 0; i < npcList.Count; i++)
            {
                //���Ʊ�Ϊ1000
                if (npcList[i].isDead)
                {
                    seatWeight.Add(MAX);
                    continue;
                }
                //δ����ѡ���
                List<CardData> allCards = new List<CardData>(dataBase.PublicCard);
                for (int j = 0; j < npcList[i].data.cards.Count; j++)
                {
                    allCards.Add(npcList[i].data.cards[j]);
                }
                List<List<CardData>> allChoose = AiSystem.GetCombinations(allCards, 5);
                int min = MAX;
                for (int j = 0; j < allChoose.Count; j++)
                {
                    int temp = MatchWeight(allChoose[j]);
                    if (temp < min)
                    {
                        min = temp;
                    }

                }
                seatWeight.Add(min);
            }
        }
    }


    public void OverBigTurn()
    {
        //����������
        int openNum = dataReader.levelDic[levelID].ShowCardNum[currentBigTurn-1];
        for(int i=0;i<dataBase.PublicCard.Count;i++)
        {
            if(dataBase.PublicCard[i].isRevert&&openNum>0)
            {
                dataBase.PublicCard[i].isRevert = false;
                openNum--;
            }
        }
        ReverAllCards();
        currentBigTurn++;

        //�Ծֽ���
        if(currentBigTurn>maxBigTurn)
        {
            turnSystem.OpenMiniCard();
            //���н���(����ѡ����������ȷ����ť)
            canBeChoose = true;
            //���ȷ����ť�󡪡�1���ֽ���2���㽱��(���ѡ��5��+ÿ��AIѡ��5��)
            SetSureBtn();
        }
        //������һ��
        else
        {
            TurnSystem.Instance.ResetTurn();
            TurnSystem.Instance.ExchangeTurn();
        }

        void SetSureBtn()
        {
            sureBtn = Instantiate(SureBtnPerfeb,gameView.transform).GetComponent<Button>();
            sureBtn.onClick.AddListener(() =>
            {
                if (selectedCard.Count > 5)
                {
                    Debug.Log("���ѡ5�ſ�");
                }
                else if (selectedCard.Count < 5)
                {
                    Debug.Log("����ѡ5�ſ�");
                }
                else
                {
                    GeneratePricePool();
                    GivePrice();
                    Destroy(sureBtn.gameObject);
                    sureBtn = null;

                }
            });
        }
    }
    public void ReverAllCards()
    {
        //������
        for(int i=0;i<dataBase.PublicCard.Count;i++)
        {
            CardData data = dataBase.PublicCard[i];
            SetRevert(i, data);
        }
        //���
        for (int i=0;i<dataBase.PlayerData.cards.Count;i++)
        {
            CardData data = dataBase.PlayerData.cards[i];
            SetRevert(i+dataBase.PublicCard.Count, data);
        }
        //npc
        for(int i=0;i<dataBase.CharacterDic.Count;i++)
        {
            NPCData npcData = dataBase.CharacterDic[i];
            //������һ����Ϊ��ת״̬��������������Ϊ��ת
            CardData data = npcData.cards[0];
            if (data.isRevert)
            {
                foreach (var obj in characterViews[i].miniCards)
                {
                    obj.SetActive(false);
                }
            }
            else
            {
                foreach (var obj in characterViews[i].miniCards)
                {
                    obj.SetActive(true);
                }
            }

        }

        void SetRevert(int i, CardData data)
        {
            if (data.isRevert)
            {
                cardViews[i].Revert(true, data.cardBack);
            }
            else
            {
                cardViews[i].Revert(false, data.cardFront);
            }
        }
    }

    private void LoadAllCard()
    {
        for (int i = 0; i < 5; i++)
        {
            JuageGetCard(i);
        }
        for (int i = 0; i < 2; i++)
        {
            PlayerGetCard(i);
        }
        foreach (var npc in DataBase.Instance.CharacterDic)
        {
            for(int i=0; i<2;i++)
            {
                NPCGetCard(npc.Key,i);
            }
        }
    }

    CardData GetCard()
    {
        int end = dataBase.CardLibrary.Count;
        int index = Random.Range(0, end);
        CardData data = DataBase.Instance.CardLibrary[index];
        DataBase.Instance.CardLibrary.RemoveAt(index);
        return data;
    }


    public void NPCGetCard(int id,int index)
    {
        CardData data = GetCard();
        DataBase.Instance.CharacterDic[id].cards.Add(data);
        characterViews[id].SetMiniCardView(data, index);
    }
    public void JuageGetCard(int index)
    {
        CardData data;
        GameObject card;
        PutCardInTable(out data, out card);
        LevelBean level = GetLevel(levelID);
        //������ݲ��ƶ�����λ��
        DataBase.Instance.PublicCard.Add(data);
        card.GetComponent<RectTransform>().anchoredPosition = new Vector2(level.PublicCardPosX[index], level.PublicCardPosY[index]);
    }
    public void PlayerGetCard(int index)
    {
        CardData data;
        GameObject card;
        PutCardInTable(out data, out card);
        LevelBean level = GetLevel(levelID);
        //������ݲ��ƶ�����λ��
        data.isRevert = false;
        DataBase.Instance.PlayerData.cards.Add(data);
        card.GetComponent<RectTransform>().anchoredPosition = new Vector2(level.OwnCardPosX[index], level.OwnCardPosY[index]);
    }

    private void PutCardInTable(out CardData data, out GameObject card)
    {
        data = GetCard();
        card = Instantiate(CardPerfebs, gameView.transform);
        CardView view = card.GetComponent<CardView>();
        view.SetView(data.num, data.num, data.colorIcon, data.colorIcon);
        view.ID = data.id;
        cardViews.Add(view);
    }

    private void InitScene()
    {
        if (playerView != null)
        {
            Destroy(playerView.gameObject);
        }
        if(gameView!=null)
        {
            Destroy(gameView.gameObject);
        }
        if (characterViews != null)
        {
            foreach (var view in characterViews)
            {
                Destroy(view.gameObject);
            }
        }
        if (cardViews != null)
        {
            foreach (var card in cardViews)
            {
                Destroy(card.gameObject);
            }
        }
        playerView = null;
        gameView = null;
        characterViews = new List<CharacterView>();
        cardViews = new List<CardView>();
        currentBigTurn = 1;
        maxBigTurn = GetLevel(levelID).ShowCardNum.Count;
    }

    public void InitCardTable()
    {
        DataBase data = DataBase.Instance;
        //��ʼ������
        data.PlayerData.poolChips = 0;
        data.PlayerData.cards = new List<CardData>();
        foreach (var npc in data.CharacterDic)
        {
            npc.Value.poolChips = 0;
            if(npc.Value.cards!=null)
            {
                npc.Value.cards.Clear();
            }

        }
        data.CardLibrary = GenerateCardLibrary(1, 52);//��ʱ����
        data.PublicCard = new List<CardData>();
        canBeChoose = false;
        selectedCard = new List<CardData>();
        pricePools = new List<PricePoolData>();

        //������Ϸ����
        playerView.SetView(data.PlayerData.leftChips.ToString(),data.PlayerData.poolChips.ToString());
        int index = 0;
        foreach(var npc in characterViews)
        {
            NPCData npcData = data.CharacterDic[index++];
            npc.SetView(npcData.name, npcData.leftChips.ToString(), npcData.poolChips.ToString());
        }
        foreach(var card in cardViews)
        {
            Destroy(card.gameObject);
        }
        cardViews = new List<CardView>();
        
    }

    /// <summary>
    /// ��ʼID������IDΪһ���ƿ�
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    private List<CardData> GenerateCardLibrary(int start,int end)
    {
        List<CardData> list = new List<CardData>();

        for (int i = start; i <= end; i++)
        {
            CardBean bean = dataReader.cardDic[i];
            CardData cardData = new CardData();
            cardData.colorType = bean.ColorType;
            cardData.num = bean.CardChar;
            cardData.id = bean.ID;
            cardData.weight = bean.Weight;
            cardData.cardBack = bean.CardBack;
            cardData.cardFront = bean.CardFront;
            cardData.colorIcon = bean.CardColor;
            list.Add(cardData);
        }
        return list;
    }

    private LevelBean GetLevel(int id)
    {
        return dataReader.levelDic[id];
    }
    private List<GameObject> GenerateNPC(LevelBean level, GameObject panel)
    {
        List<GameObject> NPCList = new List<GameObject>();
        GameObject npc = null;
        for (int i = 0; i < level.NPC.Count; i++)
        {
            if (i == 0)
            {
                npc = Instantiate(CharacterB, panel.transform);
            }
            else
            {
                npc = Instantiate(CharacterA, panel.transform);
            }
            //��������
            NPCData data = new NPCData { leftChips = level.NPCChips[i], name = level.NPC[i] };//iconδ����
            DataBase.Instance.CharacterDic.Add(i, data);
            //�����������
            npc.GetComponent<RectTransform>().anchoredPosition = new Vector2(level.NPCPosX[i], level.NPCPosY[i]);
            //����UI
            CharacterView view = npc.GetComponent<CharacterView>();
            view.SetView(data.name, data.leftChips.ToString(), data.poolChips.ToString());
            NPCList.Add(npc);
        }
        return NPCList;
    }

    private CardData SearchCardFromDB(int ID)
    {
        foreach(var card in dataBase.PublicCard)
        {
            if(ID==card.id)
            {
                return card;
            }
        }
        foreach(var card in dataBase.PlayerData.cards)
        {
            if(ID==card.id)
            {
                return card;
            }
        }
        return null;
    }
    private CardData SearchCardFromLT(int ID)
    {
        for(int i=0;i<selectedCard.Count;i++)
        {
            if(ID==selectedCard[i].id)
            {
                return selectedCard[i];
            }
        }
        return null;
    }

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem; 
    List<CardData>selectedCard=new List<CardData>();
    private void ChooseCard()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //Debug.Log("���");
            // ����һ�� PointerEventData��ʹ�õ�ǰ EventSystem
            PointerEventData pointerData = new PointerEventData(eventSystem);
            pointerData.position = Input.mousePosition; // ���õ��λ��Ϊ���λ��

            // ����һ���б�洢���߼����
            List<RaycastResult> results = new List<RaycastResult>();

            // ʹ�� GraphicRaycaster �������߼��
            raycaster.Raycast(pointerData, results);

            // ��������
            foreach (RaycastResult result in results)
            {
               if(result.gameObject.layer==LayerMask.NameToLayer("Card"))
                {
                    //Debug.Log("���ɹ�");
                    CardView view = result.gameObject.GetComponent<CardView>();
                    if(view.isSelected==false)
                    {
                        view.isSelected = true;
                        view.GetComponent<Image>().color = new Color(0.5f, 0.5f, 1f);
                        selectedCard.Add(SearchCardFromDB(view.ID));
                    }
                    else
                    {
                        view.isSelected = false;
                        view.GetComponent<Image>().color = new Color(1, 1, 1f);
                        selectedCard.Remove(SearchCardFromLT(view.ID));
                    }
                }
            }
        }
    }

    private void Update()
    {
        if(canBeChoose)
        {
            ChooseCard();
        }
        
    }
}
