using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// 매치를 찾는 클래스 정의
public class FindMatches : MonoBehaviour
{
    // 게임 보드에 대한 참조
    private Board board;
    // 현재 매치된 아이템들을 저장하는 리스트
    public List<GameObject> currentMatches = new List<GameObject>();

    // 초기화 함수
    void Start()
    {
        // Board 타입의 객체를 찾아서 할당
        board = FindObjectOfType<Board>();
    }

    // 모든 매치를 찾는 함수
    public void FindAllMatches()
    {
        // 코루틴 시작
        StartCoroutine(FindAllMatchesCo());
    }

    // 가로 폭탄이 있는지 확인하는 함수
    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        // 각 Dot의 가로 폭탄 여부를 확인하고, 매치된 리스트에 추가
        if (dot1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.row));
        }

        if (dot2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
        }

        if (dot3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
        }
        return currentDots;
    }

    // 세로 폭탄이 있는지 확인하는 함수
    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        // 각 Dot의 세로 폭탄 여부를 확인하고, 매치된 리스트에 추가
        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.column));
        }

        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
        }

        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
        }
        return currentDots;
    }

    // 매치된 리스트에 Dot을 추가하고 매치되었다고 표시하는 함수
    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    // 주변 Dot들을 매치된 리스트에 추가하는 함수
    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    // 모든 매치를 찾는 코루틴
    private IEnumerator FindAllMatchesCo()
    {
        // 0.2초 대기
        yield return new WaitForSeconds(.2f);
        // 게임 보드 전체를 순회
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];

                if (currentDot != null)
                {
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    // 가로 방향 매치 확인
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null)
                        {
                            Dot rightDotDot = rightDot.GetComponent<Dot>();
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                // 매치된 Dot들을 처리
                                currentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot));
                                currentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot));
                                GetNearbyPieces(leftDot, currentDot, rightDot);
                            }
                        }
                    }

                    // 게임 보드의 세로 길이 내에서 확인
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];

                        // 위쪽과 아래쪽 Dot이 모두 존재하는지 확인
                        if (upDot != null && downDot != null)
                        {
                            Dot downDotDot = downDot.GetComponent<Dot>();
                            Dot upDotDot = upDot.GetComponent<Dot>();

                            // 현재 Dot과 위, 아래 Dot이 같은 타입인지 확인하여 매치 처리
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                // 세로 및 가로 폭탄 확인 후 매치된 리스트에 추가
                                currentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot));
                                currentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot));

                                // 주변 Dot을 매치 리스트에 추가
                                GetNearbyPieces(upDot, currentDot, downDot);
                            }
                        }
                    }
                }
            }
        }
    }

    // 특정 색상의 모든 조각을 매치시키는 함수
    public void MatchPiecesOfColor(string color)
    {
        // 게임 보드를 순회하며 해당 색상의 Dot을 찾아 매치 처리
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (board.allDots[i, j].tag == color)
                    {
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    // 특정 열의 모든 Dot을 가져오는 함수
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        // 해당 열을 순회하며 매치된 Dot을 리스트에 추가
        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    // 특정 행의 모든 Dot을 가져오는 함수
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        // 해당 행을 순회하며 매치된 Dot을 리스트에 추가
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

    // 폭탄을 확인하고 적절한 조치를 취하는 함수
    public void CheckBombs()
    {
        // 현재 움직인 Dot이 있고 매치된 상태인지 확인
        if (board.currentDot != null)
        {
            if (board.currentDot.isMatched)
            {
                // 매치된 상태를 해제
                board.currentDot.isMatched = false;

                // Dot의 스와이프 각도에 따라 폭탄 유형 결정 및 생성
                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                    || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }

            // 다른 Dot도 매치된 경우 해당 처리
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    // 매치된 상태를 해제
                    otherDot.isMatched = false;

                    // Dot의 스와이프 각도에 따라 폭탄 유형 결정 및 생성
                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                    || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }


   /* public void CheckBombs()
    {
        //Did the player move something?
        if (board.currentDot != null)
        {
            //Is the piece they moved matched?
            if (board.currentDot.isMatched)
            {
                //make it unmatched
                board.currentDot.isMatched = false;
                //Decide what kind of bomb to make
                *//*
                int typeOfBomb = Random.Range(0, 100);
                if(typeOfBomb < 50){
                    //Make a row bomb
                    board.currentDot.MakeRowBomb();
                }else if(typeOfBomb >= 50){
                    //Make a column bomb
                    board.currentDot.MakeColumnBomb();
                }
                *//*
                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                   || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
            //Is the other piece matched?
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                //Is the other Dot matched?
                if (otherDot.isMatched)
                {
                    //Make it unmatched
                    otherDot.isMatched = false;
                    *//*
                    //Decide what kind of bomb to make
                    int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50)
                    {
                        //Make a row bomb
                        otherDot.MakeRowBomb();
                    }
                    else if (typeOfBomb >= 50)
                    {
                        //Make a column bomb
                        otherDot.MakeColumnBomb();
                    }
                    *//*
                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                   || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }

        }
    }*/

}
