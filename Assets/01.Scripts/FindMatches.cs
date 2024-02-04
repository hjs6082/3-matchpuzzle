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

                            if(currentDot.GetComponent<Dot>().isColumnBomb)
                            {
                                currentMatches.Union(GetColumnPieces(i));
                            }


                            if (leftDot.GetComponent<Dot>().isColumnBomb)
                            {
                                currentMatches.Union(GetColumnPieces(i-1));
                            }


                            if (rightDot.GetComponent<Dot>().isColumnBomb)
                            {
                                currentMatches.Union(GetColumnPieces(i + 1));
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

                            if (currentDot.GetComponent<Dot>().isRowBomb)
                            {
                                currentMatches.Union(GetRowPieces(j));
                            }


                            if (upDot.GetComponent<Dot>().isColumnBomb)
                            {
                                currentMatches.Union(GetRowPieces(j + 1));
                            }


                            if (downDot.GetComponent<Dot>().isColumnBomb)
                            {
                                currentMatches.Union(GetRowPieces(j - 1));
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


    // 폭탄이 있는지 확인하고, 있다면 폭탄을 생성하는 메서드
    public void CheckBombs()
    {
        // 현재 Dot이 존재하고 매치되었는지 확인
        if (board.currentDot != null)
        {
            // 만약 현재 Dot이 매치된 경우
            if (board.currentDot.isMatched)
            {
                // 현재 Dot의 매치 여부를 초기화
                board.currentDot.isMatched = false;

                // 랜덤으로 폭탄의 종류를 선택
                int typeOfBomb = Random.Range(0, 100);

                // 50%의 확률로 가로 폭탄 생성, 50%의 확률로 세로 폭탄 생성
                if (typeOfBomb < 50)
                {
                    // 가로 폭탄 생성 함수 호출
                    board.currentDot.MakeRowBomb();
                }
                else if (typeOfBomb >= 50)
                {
                    // 세로 폭탄 생성 함수 호출
                    board.currentDot.MakeColumnBomb();
                }
            }
            // 현재 Dot이 매치되지 않았지만, 다른 Dot이 매치된 경우 (보통은 이어진 폭탄을 생성하기 위함)
            else if (board.currentDot.otherDot != null)
            {
                // 추가적인 기능 또는 처리를 수행할 수 있음
                // (현재는 빈 상태로 두어 주석이 없습니다)
            }
        }
    }

}