using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReader : MonoBehaviour
{
    Dictionary<int, CardBean> cardDic;
    Dictionary<int, TargetBean> targetDic;
    Dictionary<int, LevelBean> levelDic;

    private void Awake()
    {
        cardDic = new Dictionary<int, CardBean>(); targetDic = new Dictionary<int, TargetBean>(); levelDic = new Dictionary<int, LevelBean>();//初始化
        //将数据读取到字典中进行存储
        List<CardBean> cardLibrary = JsonMgr.Instance.LoadData<List<CardBean>>("play_poker_card");
        List<TargetBean> targetLibrary = JsonMgr.Instance.LoadData<List<TargetBean>>("play_poker_targetType");
        List<LevelBean> levelLibrary = JsonMgr.Instance.LoadData<List<LevelBean>>("play_poker_level");
        foreach(CardBean card in cardLibrary)
        {
            cardDic.Add(card.id,card);
        }
        foreach(TargetBean target in targetLibrary)
        {
            targetDic.Add(target.id, target);
        }
        foreach(LevelBean level in levelLibrary)
        {
            levelDic.Add(level.id, level);
        }

    }
    private void Start()
    {
        //print(cardDic[1]);
        //print(targetDic[1]);
        //print(levelDic[1]);
    }
    
}
