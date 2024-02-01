using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currenState = GameState.move;
    // 게임 보드의 너비와 높이
    public int width;
    public int height;

    public int offSet;

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
                Vector2 tempPosition = new Vector2(i, j + offSet);
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
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
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

    // 일치하는 닷을 제거하는 메서드
    private void DestroyMatchesAt(int column, int row)
    {
        // 해당 위치에 일치하는 닷이 있고, 이미 매치된 상태라면 제거
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    // 보드 전체에서 일치하는 닷을 제거하는 메서드
    public void DestroyMatches()
    {
        // 보드의 모든 칸에 대해 반복하며 일치하는 닷을 제거
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    // 보드 전체에서 일치하는 닷을 제거한 후 빈 자리를 아래로 내리는 코루틴
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;

        // 모든 열에 대해 반복
        for (int i = 0; i < width; i++)
        {
            // 모든 행에 대해 반복
            for (int j = 0; j < height; j++)
            {
                // 현재 위치에 닷이 없다면 nullCount를 증가시킴
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                // 현재 위치에 닷이 있고, nullCount가 0보다 크다면
                else if (nullCount > 0)
                {
                    // 현재 위치의 닷을 nullCount만큼 아래로 내리고 해당 위치를 null로 설정
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0; // 다음 열로 이동할 때 nullCount 초기화
        }

        yield return new WaitForSeconds(.4f); // 일정 시간 동안 대기
        StartCoroutine(FillBoardCo()); // 새로운 닷으로 채우는 코루틴 호출
    }

    // 보드를 채우는 메서드
    private void RefillBoard()
    {
        // 모든 열에 대해 반복
        for (int i = 0; i < width; i++)
        {
            // 모든 행에 대해 반복
            for (int j = 0; j < height; j++)
            {
                // 현재 위치에 닷이 없다면 새로운 닷 생성하여 해당 위치에 저장
                if (allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    // 보드 전체에서 일치하는 닷이 있는지 확인하는 메서드
    private bool MatchesOnBoard()
    {
        // 모든 열에 대해 반복
        for (int i = 0; i < width; i++)
        {
            // 모든 행에 대해 반복
            for (int j = 0; j < height; j++)
            {
                // 현재 위치에 닷이 있고, 해당 닷이 일치 상태인지 확인
                if (allDots[i, j] != null && allDots[i, j].GetComponent<Dot>().isMatched)
                {
                    return true; // 일치하는 닷이 있다면 true 반환
                }
            }
        }
        return false; // 일치하는 닷이 없다면 false 반환
    }

    // 보드를 채우고 일치하는 닷을 제거하는 코루틴
    private IEnumerator FillBoardCo()
    {
        RefillBoard(); // 보드를 채우는 메서드 호출
        yield return new WaitForSeconds(.5f); // 일정 시간 동안 대기

        // 보드에서 여전히 일치하는 닷이 있다면 반복
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f); // 일정 시간 동안 대기
            DestroyMatches(); // 일치하는 닷을 제거하는 메서드 호출
        }
        yield return new WaitForSeconds(.5f);
        currenState = GameState.move;
    }
}
