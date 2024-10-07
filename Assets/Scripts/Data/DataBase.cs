using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase
{
    private static DataBase instance=new DataBase();
    public static DataBase Instance = instance;

    public Dictionary<int, NPCData> CharacterDic=new Dictionary<int, NPCData>();//场上的所有角色数据
    public List<CardData> CardLibrary=new List<CardData>();//牌库数据
    public PlayerData PlayerData;
    public List<CardData> PublicCard = new List<CardData>();//公共牌数据


    public void ReSet()
    {
        CharacterDic = new Dictionary<int, NPCData>();//场上的所有角色数据
        CardLibrary = new List<CardData>();//牌库数据
        PlayerData=null;
        PublicCard = new List<CardData>();//公共牌数据
    }

}
