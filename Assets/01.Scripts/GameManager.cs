using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score; // 현재 점수
    public int turn; // 현재 턴

    private void Awake()
    {
        //인스턴스 선언
        instance = this;
    }

    public int GetScore()
    {
        return score;
    }

    public int GetTrun()
    {
        return turn;
    }


}
