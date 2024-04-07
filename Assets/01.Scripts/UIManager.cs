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
        //턴, 스코어 업데이트
        turnText.text = GameManager.instance.turn.ToString();
        scoreText.text = GameManager.instance.score.ToString();
    }
}
