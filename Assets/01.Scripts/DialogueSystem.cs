using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���̾�α� Ÿ���� �����ϴ� Enum
public enum DialogueType
{
    OneHuman,
    TwoHumans,
    ThreeHumans
}

[System.Serializable] // Unity �ν����Ϳ��� ���� �����ϵ��� ����
public class DialogueConfig
{
    public DialogueType type; // ���̾�α� Ÿ��
    public GameObject uiPrefab; // UI ������
    public List<Sprite> humanSprites = new List<Sprite>(); // �Ҵ�� ��������Ʈ ����Ʈ
}

public class DialogueSystem : MonoBehaviour
{
    public List<DialogueConfig> configs = new List<DialogueConfig>(); // �� Ÿ�Ժ� ������ ������ ����Ʈ

    // Ư�� Ÿ���� ���̾�α� ������ �������� �޼���
    public DialogueConfig GetConfig(DialogueType type)
    {
        return configs.Find(config => config.type == type);
    }

    // ���� ��� ���
    void Start()
    {
        // TwoHumans Ÿ���� ���̾�α� ������ �����ͼ� ����ϴ� ��
        DialogueConfig config = GetConfig(DialogueType.TwoHumans);

        // config.uiPrefab�� ����Ͽ� UI ����
        // config.humanSprites�� ����Ͽ� �ʿ��� ��������Ʈ ó��
    }
}
