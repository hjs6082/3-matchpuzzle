using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //�ν��Ͻ� ����
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
        //���� �� ǥ��
        turnText.text = GameManager.instance.turn.ToString();
        //���� ���ھ� ǥ��
        scoreText.text = GameManager.instance.score.ToString();
    }
}
