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
        // ���� ���忡 ���� ���� ����
        board = FindObjectOfType<Board>();
    }

    // ��� ��ġ�� ã�� �޼��� ȣ��
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    // ��� ��ġ�� ã�� �ڷ�ƾ
    private IEnumerator FindAllMatchesCo()
    {
        // �ణ�� ������ �ְ� ��ġ ã�� ����
        yield return new WaitForSeconds(.2f);

        // ������ ��� ĭ�� ���� �ݺ�
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                // ���� ��ġ�� Dot ��������
                GameObject currentDot = board.allDots[i, j];

                // ���� ��ġ�� Dot�� �����ϴ� ���
                if (currentDot != null)
                {
                    // �¿� ���� ��ġ ���� Ȯ��
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        // �¿� �������� ��ġ�ϴ� Dot�� ���� ���
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
                                // �ߺ� �߰� ������ ���� ���ǹ�
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

                    // ���� ���� ��ġ ���� Ȯ��
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];

                        // ���� �������� ��ġ�ϴ� Dot�� ���� ���
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
                                // �ߺ� �߰� ������ ���� ���ǹ�
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


    // ��ź�� �ִ��� Ȯ���ϰ�, �ִٸ� ��ź�� �����ϴ� �޼���
    public void CheckBombs()
    {
        // ���� Dot�� �����ϰ� ��ġ�Ǿ����� Ȯ��
        if (board.currentDot != null)
        {
            // ���� ���� Dot�� ��ġ�� ���
            if (board.currentDot.isMatched)
            {
                // ���� Dot�� ��ġ ���θ� �ʱ�ȭ
                board.currentDot.isMatched = false;

                // �������� ��ź�� ������ ����
                int typeOfBomb = Random.Range(0, 100);

                // 50%�� Ȯ���� ���� ��ź ����, 50%�� Ȯ���� ���� ��ź ����
                if (typeOfBomb < 50)
                {
                    // ���� ��ź ���� �Լ� ȣ��
                    board.currentDot.MakeRowBomb();
                }
                else if (typeOfBomb >= 50)
                {
                    // ���� ��ź ���� �Լ� ȣ��
                    board.currentDot.MakeColumnBomb();
                }
            }
            // ���� Dot�� ��ġ���� �ʾ�����, �ٸ� Dot�� ��ġ�� ��� (������ �̾��� ��ź�� �����ϱ� ����)
            else if (board.currentDot.otherDot != null)
            {
                // �߰����� ��� �Ǵ� ó���� ������ �� ����
                // (����� �� ���·� �ξ� �ּ��� �����ϴ�)
            }
        }
    }

}