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
        instance = this;
    }
}
