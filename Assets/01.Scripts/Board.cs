using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // ���� ������ �ʺ�� ����
    public int width;
    public int height;

    // Ÿ�ϰ� �� �����յ�
    public GameObject tilePrefab;
    public GameObject[] dots;

    // ��� Ÿ�ϰ� ���� ������ �迭��
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;

    // Start is called before the first frame update
    void Start()
    {
        // �迭 �ʱ�ȭ �� ���� ����
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    // ���� ���� �޼���
    private void SetUp()
    {
        // ������ ��� ĭ�� ���� �ݺ�
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // ���� ��ġ (i, j)�� Ÿ���� �����ϰ� �ʱ�ȭ
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                // ���� ��ġ (i, j)�� ���� �������� �����Ͽ� �����ϰ� �ʱ�ȭ
                int dotToUse = Random.Range(0, dots.Length);
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";

                // ������ ���� �迭�� ����
                allDots[i, j] = dot;
            }
        }
    }
}
