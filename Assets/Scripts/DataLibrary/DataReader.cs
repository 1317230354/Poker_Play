using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReader : MonoBehaviour
{
    private static DataReader instance;
    public static DataReader Instance => instance;

    public Dictionary<int, CardBean> cardDic;
    public Dictionary<int, TargetBean> targetDic;
    public Dictionary<int, LevelBean> levelDic;

    private void Awake()
    {
        instance = this;
        cardDic = new Dictionary<int, CardBean>(); targetDic = new Dictionary<int, TargetBean>(); levelDic = new Dictionary<int, LevelBean>();//初始化
        //将数据读取到字典中进行存储
        List<CardBean> cardLibrary = JsonMgr.Instance.LoadData<List<CardBean>>("play_poker_card");
        List<TargetBean> targetLibrary = JsonMgr.Instance.LoadData<List<TargetBean>>("play_poker_targetType");
        List<LevelBean> levelLibrary = JsonMgr.Instance.LoadData<List<LevelBean>>("play_poker_level");
        foreach(CardBean card in cardLibrary)
        {
            cardDic.Add(card.ID,card);
        }
        foreach(TargetBean target in targetLibrary)
        {
            targetDic.Add(target.ID, target);
        }
        foreach(LevelBean level in levelLibrary)
        {
            levelDic.Add(level.ID, level);
        }

    }
    private void Start()
    {
        print(cardDic[1]);
        print(targetDic[1]);
        print(levelDic[1]);
    }
    
}
