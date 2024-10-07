using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPC
{
    public CharacterView view;
    public NPCData data;
    public bool isOverTurn;
    public bool isDead;
}
public class Player
{
    public PlayerData data;
    public PlayerView view;
    public bool isOverTurn;
    public bool isDead;
}
public class TurnSystem : MonoBehaviour
{
    private static TurnSystem instance;
    public static TurnSystem Instance => instance;

    CoreSystem coreSystem;
    DataBase dataBase;

    public GameObject ChoosePerfebA,ChoosePerfebB;

    Dictionary<int, NPC> NPCs = new Dictionary<int, NPC>();
    [HideInInspector]
    public Player player;

    int currentIndex = 0;

    GameObject choosePanel;

    public void DestroyInstance()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;  // ȷ����̬���ñ����
        }
    }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        dataBase = DataBase.Instance;
        coreSystem = CoreSystem.Instance;
    }
    //1��¼���н�ɫ����λ��(��ɫ�Ķ��󣬽�ɫ�����ݣ� 0��λΪ��ң����ఴ��˳ʱ�����
    public void SetSeat()
    {
        if(dataBase == null)
        {
            dataBase = DataBase.Instance;
        }
        if(coreSystem==null)
        {
            coreSystem = CoreSystem.Instance;
        }
        //��ʼ������
        if (NPCs == null)
        {
            NPCs = new Dictionary<int, NPC>();
        }
        else
            NPCs.Clear();

        player = new Player { view = coreSystem.playerView, data = dataBase.PlayerData };

        int num = coreSystem.characterViews.Count;
        NPC npc;
        //��ֻ����Һ���һ����ɫʱ
        if (num==1)
        {
            npc = new NPC { view = coreSystem.characterViews[0], data = dataBase.CharacterDic[0] };
            NPCs.Add(1, npc);
        }
        //��npc����Ϊ3�˼�����ʱ
        if(num<=3)
        {
            
            for (int i=1;i<num;i++)
            {
                npc = new NPC { view = coreSystem.characterViews[i], data = dataBase.CharacterDic[i] };
                NPCs.Add(i, npc);
            }
            //0����������ĩβ��λ
            npc = new NPC { view = coreSystem.characterViews[0], data = dataBase.CharacterDic[0] };
            NPCs.Add(num, npc);
        }
        //npc������3��ʱ
        if(num>3)
        {
            //����������������
            for(int i=1;i<3;i++)
            {
                npc = new NPC { view = coreSystem.characterViews[i], data = dataBase.CharacterDic[i] };
                NPCs.Add(i, npc);
            }
            //0����������3��λ
            npc = new NPC { view = coreSystem.characterViews[0], data = dataBase.CharacterDic[0] };
            NPCs.Add(3, npc);
            for(int i=3;i<num;i++)
            {
                npc = new NPC { view = coreSystem.characterViews[i], data = dataBase.CharacterDic[i] };
                NPCs.Add(i+1, npc);
            }
        }
    }

    //2��¼���н�ɫ�Ƿ������غ�
    /// <summary>
    /// ÿһ�ֿ�ʼǰ�������н�ɫ�Ļغ�״̬
    /// </summary>
    public void ResetTurn()
    {
        player.isOverTurn = false;
        foreach(var npcKey in NPCs.Keys)
        {
            NPCs[npcKey].isOverTurn = false;
        }
    }
    /// <summary>
    /// ���ý�ɫ�غ�״̬
    /// </summary>
    /// <param name="index"></param>
    public void OverTurn(int index)
    {
        if (index == 0)
        {
            player.isOverTurn = true;
        }
        else
        {
            NPCs[index].isOverTurn = true;
        }
    }

    //3�غ��л�
    /// <summary>
    /// ���õ�ǰ�غ�����
    /// </summary>
    /// <param name="index"></param>
    public void SetIndex(int index)
    {
        currentIndex = index;
    }
    public void StartTurn()
    {
        int lastIndex = FindLastPlayer(currentIndex);
        //print(lastIndex);
        //����������ɫ���ƣ�����ֽ���
        if (lastIndex == currentIndex)
        {
            coreSystem.OverBigTurn();
        }
        //Ϊ��ң�����ѡ��
        if (currentIndex == 0)
        {
            if(player.isDead)
            {
                //Debug.Log("�������");
                //�л�����һλ��ɫ
                ExchangeTurn();
            }
            //���û�н��й��غϣ���ʣ�����Ϊ0,Ĭ�������غ�
            if (player.isOverTurn == false && player.data.leftChips == 0)
            {
                player.isOverTurn = true;
                ExchangeTurn();
            }
            else if(player.isOverTurn&&isSame(lastIndex))
            {
                //Debug.Log("��ҳ�����ͬ����һ��");
                //�����һλ��ɫ�͸ý�ɫ������С��ͬ,�ҵ�ǰ��ɫ���й��غ�,������ǰ��
                coreSystem.OverBigTurn();
            }
            else
            {
                if (isSame(lastIndex))
                {
                    choosePanel = Instantiate(ChoosePerfebB, coreSystem.gameView.transform);
                }
                else if (NPCs[lastIndex].data.poolChips > player.data.poolChips)
                {
                    choosePanel = Instantiate(ChoosePerfebA, coreSystem.gameView.transform);
                }
                else if (NPCs[lastIndex].data.poolChips < player.data.poolChips)
                {
                    //Debug.Log("��ҳ������");
                    //������ǰ��
                    coreSystem.OverBigTurn();
                }
            }
        }
        //Ϊnpc���ȴ�1s���Զ�ѡ��ѡ��
        else
        {
            //�ȱ�ɫ�������غ��л�
            NPCs[currentIndex].view.ChangeColor();

            if (NPCs[currentIndex].isDead)
            {
                NPCs[currentIndex].view.ChangeColor(false, true);//��ɫ
                ExchangeTurn();
            }
            else if (NPCs[currentIndex].isOverTurn == false && NPCs[currentIndex].data.leftChips == 0)
            {
                NPCs[currentIndex].isOverTurn = true;
                ExchangeTurn();
            }
            else if (NPCs[currentIndex].isOverTurn &&isSame(lastIndex))
            {
                coreSystem.OverBigTurn();
            }
            else
            {
                print("��ǰ����:"+currentIndex +"��һ������:" +lastIndex);
                //�����ǰ������������һλ��ɫ
                if (isSame(lastIndex))
                {
                    StartCoroutine(WaitOneSecond(AiSystem.PlanA,GetCurrentNPC()));
                }
                else
                {
                    //�����һ�������
                    if(lastIndex==0&& NPCs[currentIndex].data.poolChips < player.data.poolChips)
                    {
                        StartCoroutine(WaitOneSecond(AiSystem.PlanB, GetCurrentNPC()));
                        
                    }
                    else if(lastIndex!=0&&NPCs[currentIndex].data.poolChips < NPCs[lastIndex].data.poolChips)
                    {
                        StartCoroutine(WaitOneSecond(AiSystem.PlanB, GetCurrentNPC()));
                    }
                    else
                    {
                        coreSystem.OverBigTurn();
                    }
                }
               
                
            }

        }
    }

    int FindLastPlayer(int index)
    {
        int num = NPCs.Count;
        //����
        if(index>NPCs.Count||index<0)
        {
            Debug.Log("����");
            return -1;
        }
        //���ѭ��
        else if(index==0)
        {
            for(int i=num;i>0;i--)
            {
                if (NPCs[i].isDead == false)
                {
                    return i;
                }
            }
            Debug.Log("����������ɫ��������");
            return -1;
        }
        //NPC˫��ѭ��
        else
        {
            
            for(int i=index-1;i>0;i--)
            {
                if (NPCs[i].isDead==false)
                {
                    return i;
                }
            }
            if(player.isDead == false)
            {
                return 0;
            }
            for(int i=num;i>index;i--)
            {
                if (NPCs[i].isDead==false)
                {
                    return i;
                }
            }
            Debug.Log("����������ɫ��������");
            return -1;
        }
    }
    /// <summary>
    /// ����һλ��ɫ���бȽϣ��жϳ����Ƿ���ͬ
    /// </summary>
    /// <param name="lastIndex"></param>
    /// <returns></returns>
    bool isSame(int lastIndex)
    {
        //���
        if(currentIndex==0)
        {
            if (player.data.poolChips == NPCs[lastIndex].data.poolChips || player.data.leftChips==0)
            {
                return true;
            }
            return false;
        }
        //npc
        else
        {
            if (lastIndex == 0)
            {
                if (NPCs[currentIndex].data.poolChips == player.data.poolChips || NPCs[currentIndex].data.leftChips ==0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (NPCs[currentIndex].data.poolChips == NPCs[lastIndex].data.poolChips || NPCs[currentIndex].data.leftChips==0)
                {
                    return true;
                }
                return false;
            }
        }
    }
    public void ExchangeTurn()
    {
        //��ɫ
        if(currentIndex!=0)
        {
            NPCs[currentIndex].view.ChangeColor(true);
            if(NPCs[currentIndex].isDead==true)
            {
                NPCs[currentIndex].view.ChangeColor(false, true);
            }
        }
        //�߼�
        currentIndex++;
        if(currentIndex>NPCs.Count)
        {
            currentIndex = 0;
        }
        StartTurn();
    }

    public NPC GetCurrentNPC()
    {
        return NPCs[currentIndex];
    }
    //4��������
    public int CalculateGap(int indexCurrent)
    {
        if (indexCurrent > NPCs.Count)
        {
            Debug.Log("����");
            return 0;
        }
        int lastPlayer = FindLastPlayer(indexCurrent);
        if(lastPlayer==0)
        {
            return NPCs[indexCurrent].data.poolChips-player.data.poolChips;
        }
        else if(indexCurrent==0)
        {
            return player.data.poolChips - NPCs[lastPlayer].data.poolChips;
        }
        else
        {
            return NPCs[indexCurrent].data.poolChips - NPCs[lastPlayer].data.poolChips;
        }
    }

    public int CalculateGap(NPC npc)
    {
        int indexCurrent=0;
        foreach(var npcInDic in NPCs)
        {
            if (NPCs[npcInDic.Key]==npc)
            {
                indexCurrent = npcInDic.Key;
                break;
            }
        }
        if (indexCurrent == 0)
        {
            Debug.Log("����");
            return -1;
        }

        int lastPlayer = FindLastPlayer(indexCurrent);
        if (lastPlayer == 0)
        {
            return NPCs[indexCurrent].data.poolChips - player.data.poolChips;
        }
        else
        {
            return NPCs[indexCurrent].data.poolChips - NPCs[lastPlayer].data.poolChips;
        }

    }


    IEnumerator WaitOneSecond(UnityAction<NPC> action,NPC npc)
    {
        yield return new WaitForSeconds(1f);
        //����ѡ��(AI)
        action.Invoke(npc);
    }
    /// <summary>
    /// ����ǰ��������ʾ
    /// </summary>
    public void UpdatePanel()
    {
        foreach(var npc in NPCs)
        {
            NPCData data = npc.Value.data;
            npc.Value.view.UpdataView(data.leftChips.ToString(), data.poolChips.ToString());
        }
        player.view.SetView(player.data.leftChips.ToString(), player.data.poolChips.ToString());
    }

    public void OpenMiniCard()
    {
        foreach(var npc in NPCs)
        {
            npc.Value.view.ShowMiniCard();
        }
    }

    public List<NPC> GetAllNpc()
    {
        List<NPC> npcList = new List<NPC>();
        for(int i = 1; i <= NPCs.Count; i++)
        {
            npcList.Add(NPCs[i]);
        }
        return npcList;
    }    

}
