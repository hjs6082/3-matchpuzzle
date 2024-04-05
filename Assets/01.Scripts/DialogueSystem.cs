using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 다이얼로그 타입을 정의하는 Enum
public enum DialogueType
{
    OneHuman,
    TwoHumans,
    ThreeHumans
}

[System.Serializable] // Unity 인스펙터에서 편집 가능하도록 설정
public class DialogueConfig
{
    public DialogueType type; // 다이얼로그 타입
    public GameObject uiPrefab; // UI 프리팹
    public List<Sprite> humanSprites = new List<Sprite>(); // 할당될 스프라이트 리스트
}

public class DialogueSystem : MonoBehaviour
{
    public List<DialogueConfig> configs = new List<DialogueConfig>(); // 각 타입별 설정을 저장할 리스트

    // 특정 타입의 다이얼로그 설정을 가져오는 메서드
    public DialogueConfig GetConfig(DialogueType type)
    {
        return configs.Find(config => config.type == type);
    }

    // 예시 사용 방법
    void Start()
    {
        // TwoHumans 타입의 다이얼로그 설정을 가져와서 사용하는 예
        DialogueConfig config = GetConfig(DialogueType.TwoHumans);

        // config.uiPrefab을 사용하여 UI 생성
        // config.humanSprites를 사용하여 필요한 스프라이트 처리
    }
}
