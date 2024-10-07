using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int leftChips;
    public int poolChips;
    public List<CardData> cards=new List<CardData>();
    public int selectChips;//通过slider选择的筹码数量
}
