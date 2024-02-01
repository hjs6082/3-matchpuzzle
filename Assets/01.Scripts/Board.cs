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
    // ���� ������ �ʺ�� ����
    public int width;
    public int height;

    public int offSet;

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
                Vector2 tempPosition = new Vector2(i, j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                // ���� ��ġ (i, j)�� ���� �������� �����Ͽ� �����ϰ� �ʱ�ȭ
                int dotToUse = Random.Range(0, dots.Length);
                int maxIterations = 0;

                // �̹� ��ġ�ϴ� ���� �ִ��� Ȯ���ϰ�, ���� ������ ���ο� ���� ����
                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }
                maxIterations = 0;

                // ���ο� �� ���� �� �迭�� ����
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }

    // ���� ��ġ���� ��ġ�ϴ� ���� �ִ��� Ȯ���ϴ� �޼���
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

    // ��ġ�ϴ� ���� �����ϴ� �޼���
    private void DestroyMatchesAt(int column, int row)
    {
        // �ش� ��ġ�� ��ġ�ϴ� ���� �ְ�, �̹� ��ġ�� ���¶�� ����
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    // ���� ��ü���� ��ġ�ϴ� ���� �����ϴ� �޼���
    public void DestroyMatches()
    {
        // ������ ��� ĭ�� ���� �ݺ��ϸ� ��ġ�ϴ� ���� ����
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

    // ���� ��ü���� ��ġ�ϴ� ���� ������ �� �� �ڸ��� �Ʒ��� ������ �ڷ�ƾ
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;

        // ��� ���� ���� �ݺ�
        for (int i = 0; i < width; i++)
        {
            // ��� �࿡ ���� �ݺ�
            for (int j = 0; j < height; j++)
            {
                // ���� ��ġ�� ���� ���ٸ� nullCount�� ������Ŵ
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                // ���� ��ġ�� ���� �ְ�, nullCount�� 0���� ũ�ٸ�
                else if (nullCount > 0)
                {
                    // ���� ��ġ�� ���� nullCount��ŭ �Ʒ��� ������ �ش� ��ġ�� null�� ����
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0; // ���� ���� �̵��� �� nullCount �ʱ�ȭ
        }

        yield return new WaitForSeconds(.4f); // ���� �ð� ���� ���
        StartCoroutine(FillBoardCo()); // ���ο� ������ ä��� �ڷ�ƾ ȣ��
    }

    // ���带 ä��� �޼���
    private void RefillBoard()
    {
        // ��� ���� ���� �ݺ�
        for (int i = 0; i < width; i++)
        {
            // ��� �࿡ ���� �ݺ�
            for (int j = 0; j < height; j++)
            {
                // ���� ��ġ�� ���� ���ٸ� ���ο� �� �����Ͽ� �ش� ��ġ�� ����
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

    // ���� ��ü���� ��ġ�ϴ� ���� �ִ��� Ȯ���ϴ� �޼���
    private bool MatchesOnBoard()
    {
        // ��� ���� ���� �ݺ�
        for (int i = 0; i < width; i++)
        {
            // ��� �࿡ ���� �ݺ�
            for (int j = 0; j < height; j++)
            {
                // ���� ��ġ�� ���� �ְ�, �ش� ���� ��ġ �������� Ȯ��
                if (allDots[i, j] != null && allDots[i, j].GetComponent<Dot>().isMatched)
                {
                    return true; // ��ġ�ϴ� ���� �ִٸ� true ��ȯ
                }
            }
        }
        return false; // ��ġ�ϴ� ���� ���ٸ� false ��ȯ
    }

    // ���带 ä��� ��ġ�ϴ� ���� �����ϴ� �ڷ�ƾ
    private IEnumerator FillBoardCo()
    {
        RefillBoard(); // ���带 ä��� �޼��� ȣ��
        yield return new WaitForSeconds(.5f); // ���� �ð� ���� ���

        // ���忡�� ������ ��ġ�ϴ� ���� �ִٸ� �ݺ�
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f); // ���� �ð� ���� ���
            DestroyMatches(); // ��ġ�ϴ� ���� �����ϴ� �޼��� ȣ��
        }
        yield return new WaitForSeconds(.5f);
        currenState = GameState.move;
    }
}
