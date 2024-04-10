using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score; // ���� ����
    public int turn; // ���� ��

    private void Awake()
    {
        //�ν��Ͻ� ����
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
