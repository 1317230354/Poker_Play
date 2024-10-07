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
            instance = null;  // 确保静态引用被清空
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
    //1记录所有角色的座位号(角色的对象，角色的数据） 0号位为玩家，其余按照顺时针计数
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
        //初始化数据
        if (NPCs == null)
        {
            NPCs = new Dictionary<int, NPC>();
        }
        else
            NPCs.Clear();

        player = new Player { view = coreSystem.playerView, data = dataBase.PlayerData };

        int num = coreSystem.characterViews.Count;
        NPC npc;
        //当只有玩家和另一个角色时
        if (num==1)
        {
            npc = new NPC { view = coreSystem.characterViews[0], data = dataBase.CharacterDic[0] };
            NPCs.Add(1, npc);
        }
        //当npc人数为3人及以下时
        if(num<=3)
        {
            
            for (int i=1;i<num;i++)
            {
                npc = new NPC { view = coreSystem.characterViews[i], data = dataBase.CharacterDic[i] };
                NPCs.Add(i, npc);
            }
            //0号数据置于末尾座位
            npc = new NPC { view = coreSystem.characterViews[0], data = dataBase.CharacterDic[0] };
            NPCs.Add(num, npc);
        }
        //npc人数》3人时
        if(num>3)
        {
            //其余数据正常放置
            for(int i=1;i<3;i++)
            {
                npc = new NPC { view = coreSystem.characterViews[i], data = dataBase.CharacterDic[i] };
                NPCs.Add(i, npc);
            }
            //0号数据置于3号位
            npc = new NPC { view = coreSystem.characterViews[0], data = dataBase.CharacterDic[0] };
            NPCs.Add(3, npc);
            for(int i=3;i<num;i++)
            {
                npc = new NPC { view = coreSystem.characterViews[i], data = dataBase.CharacterDic[i] };
                NPCs.Add(i+1, npc);
            }
        }
    }

    //2记录所有角色是否开启过回合
    /// <summary>
    /// 每一轮开始前重置所有角色的回合状态
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
    /// 设置角色回合状态
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

    //3回合切换
    /// <summary>
    /// 设置当前回合索引
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
        //所有其它角色弃牌，则该轮结束
        if (lastIndex == currentIndex)
        {
            coreSystem.OverBigTurn();
        }
        //为玩家，加载选项
        if (currentIndex == 0)
        {
            if(player.isDead)
            {
                //Debug.Log("玩家阵亡");
                //切换到下一位角色
                ExchangeTurn();
            }
            //玩家没有进行过回合，且剩余筹码为0,默认跳过回合
            if (player.isOverTurn == false && player.data.leftChips == 0)
            {
                player.isOverTurn = true;
                ExchangeTurn();
            }
            else if(player.isOverTurn&&isSame(lastIndex))
            {
                //Debug.Log("玩家筹码相同于上一个");
                //如果上一位角色和该角色筹码差大小相同,且当前角色进行过回合,结束当前轮
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
                    //Debug.Log("玩家筹码更多");
                    //结束当前轮
                    coreSystem.OverBigTurn();
                }
            }
        }
        //为npc，等待1s后自动选择选项
        else
        {
            //先变色，表明回合切换
            NPCs[currentIndex].view.ChangeColor();

            if (NPCs[currentIndex].isDead)
            {
                NPCs[currentIndex].view.ChangeColor(false, true);//变色
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
                print("当前索引:"+currentIndex +"上一个索引:" +lastIndex);
                //如果当前筹码池相等于上一位角色
                if (isSame(lastIndex))
                {
                    StartCoroutine(WaitOneSecond(AiSystem.PlanA,GetCurrentNPC()));
                }
                else
                {
                    //如果上一个是玩家
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
        //排障
        if(index>NPCs.Count||index<0)
        {
            Debug.Log("超长");
            return -1;
        }
        //玩家循环
        else if(index==0)
        {
            for(int i=num;i>0;i--)
            {
                if (NPCs[i].isDead == false)
                {
                    return i;
                }
            }
            Debug.Log("所有其它角色都弃牌了");
            return -1;
        }
        //NPC双向循环
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
            Debug.Log("所有其它角色都弃牌了");
            return -1;
        }
    }
    /// <summary>
    /// 与上一位角色进行比较，判断筹码是否相同
    /// </summary>
    /// <param name="lastIndex"></param>
    /// <returns></returns>
    bool isSame(int lastIndex)
    {
        //玩家
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
        //变色
        if(currentIndex!=0)
        {
            NPCs[currentIndex].view.ChangeColor(true);
            if(NPCs[currentIndex].isDead==true)
            {
                NPCs[currentIndex].view.ChangeColor(false, true);
            }
        }
        //逻辑
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
    //4计算筹码差
    public int CalculateGap(int indexCurrent)
    {
        if (indexCurrent > NPCs.Count)
        {
            Debug.Log("超长");
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
            Debug.Log("错误");
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
        //进行选择(AI)
        action.Invoke(npc);
    }
    /// <summary>
    /// 更新前端数据显示
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
