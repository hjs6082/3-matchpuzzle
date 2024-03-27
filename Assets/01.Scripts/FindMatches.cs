using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// ��ġ�� ã�� Ŭ���� ����
public class FindMatches : MonoBehaviour
{
    // ���� ���忡 ���� ����
    private Board board;
    // ���� ��ġ�� �����۵��� �����ϴ� ����Ʈ
    public List<GameObject> currentMatches = new List<GameObject>();

    // �ʱ�ȭ �Լ�
    void Start()
    {
        // Board Ÿ���� ��ü�� ã�Ƽ� �Ҵ�
        board = FindObjectOfType<Board>();
    }

    // ��� ��ġ�� ã�� �Լ�
    public void FindAllMatches()
    {
        // �ڷ�ƾ ����
        StartCoroutine(FindAllMatchesCo());
    }

    // ���� ��ź�� �ִ��� Ȯ���ϴ� �Լ�
    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        // �� Dot�� ���� ��ź ���θ� Ȯ���ϰ�, ��ġ�� ����Ʈ�� �߰�
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

    // ���� ��ź�� �ִ��� Ȯ���ϴ� �Լ�
    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        // �� Dot�� ���� ��ź ���θ� Ȯ���ϰ�, ��ġ�� ����Ʈ�� �߰�
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

    // ��ġ�� ����Ʈ�� Dot�� �߰��ϰ� ��ġ�Ǿ��ٰ� ǥ���ϴ� �Լ�
    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    // �ֺ� Dot���� ��ġ�� ����Ʈ�� �߰��ϴ� �Լ�
    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    // ��� ��ġ�� ã�� �ڷ�ƾ
    private IEnumerator FindAllMatchesCo()
    {
        // 0.2�� ���
        yield return new WaitForSeconds(.2f);
        // ���� ���� ��ü�� ��ȸ
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];

                if (currentDot != null)
                {
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    // ���� ���� ��ġ Ȯ��
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
                                // ��ġ�� Dot���� ó��
                                currentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot));
                                currentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot));
                                GetNearbyPieces(leftDot, currentDot, rightDot);
                            }
                        }
                    }

                    // ���� ������ ���� ���� ������ Ȯ��
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];

                        // ���ʰ� �Ʒ��� Dot�� ��� �����ϴ��� Ȯ��
                        if (upDot != null && downDot != null)
                        {
                            Dot downDotDot = downDot.GetComponent<Dot>();
                            Dot upDotDot = upDot.GetComponent<Dot>();

                            // ���� Dot�� ��, �Ʒ� Dot�� ���� Ÿ������ Ȯ���Ͽ� ��ġ ó��
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                // ���� �� ���� ��ź Ȯ�� �� ��ġ�� ����Ʈ�� �߰�
                                currentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot));
                                currentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot));

                                // �ֺ� Dot�� ��ġ ����Ʈ�� �߰�
                                GetNearbyPieces(upDot, currentDot, downDot);
                            }
                        }
                    }
                }
            }
        }
    }

    // Ư�� ������ ��� ������ ��ġ��Ű�� �Լ�
    public void MatchPiecesOfColor(string color)
    {
        // ���� ���带 ��ȸ�ϸ� �ش� ������ Dot�� ã�� ��ġ ó��
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

    // Ư�� ���� ��� Dot�� �������� �Լ�
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        // �ش� ���� ��ȸ�ϸ� ��ġ�� Dot�� ����Ʈ�� �߰�
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

    // Ư�� ���� ��� Dot�� �������� �Լ�
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        // �ش� ���� ��ȸ�ϸ� ��ġ�� Dot�� ����Ʈ�� �߰�
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

    // ��ź�� Ȯ���ϰ� ������ ��ġ�� ���ϴ� �Լ�
    public void CheckBombs()
    {
        // ���� ������ Dot�� �ְ� ��ġ�� �������� Ȯ��
        if (board.currentDot != null)
        {
            if (board.currentDot.isMatched)
            {
                // ��ġ�� ���¸� ����
                board.currentDot.isMatched = false;

                // Dot�� �������� ������ ���� ��ź ���� ���� �� ����
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

            // �ٸ� Dot�� ��ġ�� ��� �ش� ó��
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    // ��ġ�� ���¸� ����
                    otherDot.isMatched = false;

                    // Dot�� �������� ������ ���� ��ź ���� ���� �� ����
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
