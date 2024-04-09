using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //인스턴스 선언
    public static UIManager instance;
    [SerializeField]
    private TextMeshProUGUI turnText;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;   
    }

    // Update is called once per frame
    void Update()
    {
        //남은 턴 표시
        turnText.text = GameManager.instance.turn.ToString();
        //남은 스코어 표시
        scoreText.text = GameManager.instance.score.ToString();
    }
}
