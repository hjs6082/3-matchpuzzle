using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // 게임 보드에 대한 참조 설정
        board = FindObjectOfType<Board>();
    }

    // 모든 매치를 찾는 메서드 호출
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    // 모든 매치를 찾는 코루틴
    private IEnumerator FindAllMatchesCo()
    {
        // 약간의 지연을 주고 매치 찾기 시작
        yield return new WaitForSeconds(.2f);

        // 보드의 모든 칸에 대해 반복
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                // 현재 위치의 Dot 가져오기
                GameObject currentDot = board.allDots[i, j];

                // 현재 위치에 Dot이 존재하는 경우
                if (currentDot != null)
                {
                    // 좌우 방향 매치 여부 확인
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        // 좌우 방향으로 일치하는 Dot이 있을 경우
                        if (leftDot != null && rightDot != null)
                        {
                            if(currentDot.GetComponent<Dot>().isRowBomb ||
                                leftDot.GetComponent<Dot>().isRowBomb ||
                                rightDot.GetComponent<Dot>().isRowBomb)
                            {
                                currentMatches.Union(GetRowPieces(j));
                            }

                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                // 중복 추가 방지를 위한 조건문
                                if (!currentMatches.Contains(leftDot))
                                    currentMatches.Add(leftDot);

                                leftDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(rightDot))
                                    currentMatches.Add(rightDot);

                                rightDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(currentDot))
                                    currentMatches.Add(currentDot);

                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }

                    // 상하 방향 매치 여부 확인
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];

                        // 상하 방향으로 일치하는 Dot이 있을 경우
                        if (upDot != null && downDot != null)
                        {
                            if (currentDot.GetComponent<Dot>().isColumnBomb ||
                                upDot.GetComponent<Dot>().isColumnBomb ||
                                downDot.GetComponent<Dot>().isColumnBomb)
                            {
                                currentMatches.Union(GetColumnPieces(i));
                            }

                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                // 중복 추가 방지를 위한 조건문
                                if (!currentMatches.Contains(upDot))
                                    currentMatches.Add(upDot);

                                upDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(downDot))
                                    currentMatches.Add(downDot);

                                downDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(currentDot))
                                    currentMatches.Add(currentDot);

                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i = 0; i < board.height; i++)
        {
            if(board.allDots[column,i] != null)
            {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                dots.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }
}