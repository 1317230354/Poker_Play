using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase
{
    private static DataBase instance=new DataBase();
    public static DataBase Instance = instance;

    public Dictionary<int, NPCData> CharacterDic=new Dictionary<int, NPCData>();//���ϵ����н�ɫ����
    public List<CardData> CardLibrary=new List<CardData>();//�ƿ�����
    public PlayerData PlayerData;
    public List<CardData> PublicCard = new List<CardData>();//����������


    public void ReSet()
    {
        CharacterDic = new Dictionary<int, NPCData>();//���ϵ����н�ɫ����
        CardLibrary = new List<CardData>();//�ƿ�����
        PlayerData=null;
        PublicCard = new List<CardData>();//����������
    }

}
