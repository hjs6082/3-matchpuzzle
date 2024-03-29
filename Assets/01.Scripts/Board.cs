using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 타일이 움직일동안 다른 타일을 건들지 못하게 하기 위한 Enum
public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    // 게임 보드의 너비와 높이
    public int width;
    public int height;

    public int offSet;

    // 타일과 닷 프리팹들
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;

    // 모든 타일과 닷을 저장할 배열들
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;

    // 게임이 시작하기 전에 호출되는 함수
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>(); // FindMatches 타입의 객체를 찾아서 할당
        allTiles = new BackgroundTile[width, height]; // 배경 타일 배열을 초기화
        allDots = new GameObject[width, height]; // Dot 게임 오브젝트 배열을 초기화
        SetUp(); // 게임 보드 설정 함수 호출
    }

    // 게임 보드를 설정하는 함수
    private void SetUp()
    {
        // 보드의 모든 위치에 대해 반복
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet); // Dot을 배치할 위치 계산
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject; // 배경 타일 생성
                backgroundTile.transform.parent = this.transform; // 배경 타일의 부모를 현재 객체로 설정
                backgroundTile.name = "( " + i + ", " + j + " )"; // 타일의 이름 설정

                int dotToUse = Random.Range(0, dots.Length); // 사용할 Dot의 인덱스를 무작위로 선택

                // 생성할 Dot이 이전에 생성한 Dot과 매치되는지 확인하여 매치되지 않을 때까지 반복
                int maxIterations = 0;
                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }
                maxIterations = 0;

                // Dot 게임 오브젝트 생성 및 설정
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot; // 생성된 Dot을 배열에 저장
            }
        }
    }


    // 특정 위치에서 매치가 있는지 확인하는 함수
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        // 주어진 위치의 양 옆이나 위아래에 같은 태그를 가진 Dot이 2개 있는지 확인
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag ||
                allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1 && allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag ||
                column > 1 && allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
        }
        return false; // 매치가 없으면 false 반환
    }

    // 특정 위치의 매치된 Dot을 제거하는 함수
    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            // 매치된 Dot이 특정 개수에 도달하면 폭탄 체크 함수 호출
            if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            {
                findMatches.CheckBombs();
            }

            // 파괴 효과 생성
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f); // 파티클 효과를 0.5초 후에 제거
            Destroy(allDots[column, row]); // 해당 Dot 제거
            allDots[column, row] = null; // 배열에서도 해당 위치의 Dot을 null로 설정
        }
    }

    // 보드에서 매치된 모든 Dots를 제거하는 함수
    public void DestroyMatches()
    {
        // 보드 전체를 순회하며 매치된 Dot이 있으면 제거
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
        GameManager.instance.score += 30;
        findMatches.currentMatches.Clear(); // 현재 매치 리스트를 비움
        StartCoroutine(DecreaseRowCo()); // Dot들을 아래로 이동시키는 코루틴 시작
    }


    // Dot들을 아래로 이동시키는 코루틴
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0; // null인 Dot의 수를 세는 변수
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null) // 만약 Dot이 null이면
                {
                    nullCount++; // null 카운트 증가
                }
                else if (nullCount > 0) // null이 아니고, null 카운트가 0보다 크면
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount; // Dot을 아래로 이동
                    allDots[i, j] = null; // 이동 후의 위치를 null로 설정
                }
            }
            nullCount = 0; // 다음 열로 넘어갈 때 null 카운트 초기화
        }
        yield return new WaitForSeconds(.4f); // 0.4초 대기
        StartCoroutine(FillBoardCo()); // 보드를 다시 채우는 코루틴 시작
    }


    // 보드를 다시 채우는 함수
    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null) // 만약 Dot이 null이라면
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet); // 새로운 Dot의 위치 계산
                    int dotToUse = Random.Range(0, dots.Length); // 사용할 Dot의 인덱스 무작위 선택
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity); // Dot 생성
                    allDots[i, j] = piece; // 생성된 Dot을 배열에 저장
                    piece.GetComponent<Dot>().row = j; // Dot의 행 설정
                    piece.GetComponent<Dot>().column = i; // Dot의 열 설정
                }
            }
        }
    }

    // 보드에 매치가 있는지 확인하는 함수
    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null && allDots[i, j].GetComponent<Dot>().isMatched) // 매치된 Dot이 있는지 확인
                {
                    return true; // 매치가 있다면 true 반환
                }
            }
        }
        return false; // 매치가 없다면 false 반환
    }


    // 보드를 다시 채우고 매치를 확인하는 코루틴
    private IEnumerator FillBoardCo()
    {
        RefillBoard(); // 보드를 다시 채움
        yield return new WaitForSeconds(.5f); // 약간의 딜레이를 준 후

        // 보드 상의 매치들을 처리하는 동안 반복
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f); // 매치가 처리되는 동안 기다림
            DestroyMatches(); // 매치된 Dot들을 파괴
        }
        findMatches.currentMatches.Clear(); // 현재 매치 목록을 클리어
        currentDot = null; // 현재 선택된 Dot을 리셋
        yield return new WaitForSeconds(.5f); // 추가적인 딜레이 후
        currentState = GameState.move; // 게임 상태를 '움직임'으로 변경
    }
}
