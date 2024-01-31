using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // 게임 보드의 너비와 높이
    public int width;
    public int height;

    // 타일과 닷 프리팹들
    public GameObject tilePrefab;
    public GameObject[] dots;

    // 모든 타일과 닷을 저장할 배열들
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;

    // Start is called before the first frame update
    void Start()
    {
        // 배열 초기화 및 보드 설정
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    // 보드 설정 메서드
    private void SetUp()
    {
        // 보드의 모든 칸에 대해 반복
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // 현재 위치 (i, j)에 타일을 생성하고 초기화
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                // 현재 위치 (i, j)에 닷을 랜덤으로 선택하여 생성하고 초기화
                int dotToUse = Random.Range(0, dots.Length);
                int maxIterations = 0;

                // 이미 일치하는 닷이 있는지 확인하고, 없을 때까지 새로운 닷을 선택
                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }
                maxIterations = 0;

                // 새로운 닷 생성 및 배열에 저장
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }

    // 현재 위치에서 일치하는 닷이 있는지 확인하는 메서드
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
