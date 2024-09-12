using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBean
{
    public int ID;
    public List<int> CardPublic;
    public List<float> PublicCardPosX,PublicCardPosY;
    public string CardTable;
    public List<int> CardOwn;
    public List<float> OwnCardPosX,OwnCardPosY;
    public int CardMaxNum;
    public List<int> CardTargetType;
    public int ChallengeTime;
    public List<int> LockCard;

}
