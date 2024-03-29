using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ÿ���� �����ϵ��� �ٸ� Ÿ���� �ǵ��� ���ϰ� �ϱ� ���� Enum
public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    // ���� ������ �ʺ�� ����
    public int width;
    public int height;

    public int offSet;

    // Ÿ�ϰ� �� �����յ�
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;

    // ��� Ÿ�ϰ� ���� ������ �迭��
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;

    // ������ �����ϱ� ���� ȣ��Ǵ� �Լ�
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>(); // FindMatches Ÿ���� ��ü�� ã�Ƽ� �Ҵ�
        allTiles = new BackgroundTile[width, height]; // ��� Ÿ�� �迭�� �ʱ�ȭ
        allDots = new GameObject[width, height]; // Dot ���� ������Ʈ �迭�� �ʱ�ȭ
        SetUp(); // ���� ���� ���� �Լ� ȣ��
    }

    // ���� ���带 �����ϴ� �Լ�
    private void SetUp()
    {
        // ������ ��� ��ġ�� ���� �ݺ�
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet); // Dot�� ��ġ�� ��ġ ���
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject; // ��� Ÿ�� ����
                backgroundTile.transform.parent = this.transform; // ��� Ÿ���� �θ� ���� ��ü�� ����
                backgroundTile.name = "( " + i + ", " + j + " )"; // Ÿ���� �̸� ����

                int dotToUse = Random.Range(0, dots.Length); // ����� Dot�� �ε����� �������� ����

                // ������ Dot�� ������ ������ Dot�� ��ġ�Ǵ��� Ȯ���Ͽ� ��ġ���� ���� ������ �ݺ�
                int maxIterations = 0;
                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }
                maxIterations = 0;

                // Dot ���� ������Ʈ ���� �� ����
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot; // ������ Dot�� �迭�� ����
            }
        }
    }


    // Ư�� ��ġ���� ��ġ�� �ִ��� Ȯ���ϴ� �Լ�
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        // �־��� ��ġ�� �� ���̳� ���Ʒ��� ���� �±׸� ���� Dot�� 2�� �ִ��� Ȯ��
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
        return false; // ��ġ�� ������ false ��ȯ
    }

    // Ư�� ��ġ�� ��ġ�� Dot�� �����ϴ� �Լ�
    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            // ��ġ�� Dot�� Ư�� ������ �����ϸ� ��ź üũ �Լ� ȣ��
            if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            {
                findMatches.CheckBombs();
            }

            // �ı� ȿ�� ����
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f); // ��ƼŬ ȿ���� 0.5�� �Ŀ� ����
            Destroy(allDots[column, row]); // �ش� Dot ����
            allDots[column, row] = null; // �迭������ �ش� ��ġ�� Dot�� null�� ����
        }
    }

    // ���忡�� ��ġ�� ��� Dots�� �����ϴ� �Լ�
    public void DestroyMatches()
    {
        // ���� ��ü�� ��ȸ�ϸ� ��ġ�� Dot�� ������ ����
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
        findMatches.currentMatches.Clear(); // ���� ��ġ ����Ʈ�� ���
        StartCoroutine(DecreaseRowCo()); // Dot���� �Ʒ��� �̵���Ű�� �ڷ�ƾ ����
    }


    // Dot���� �Ʒ��� �̵���Ű�� �ڷ�ƾ
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0; // null�� Dot�� ���� ���� ����
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null) // ���� Dot�� null�̸�
                {
                    nullCount++; // null ī��Ʈ ����
                }
                else if (nullCount > 0) // null�� �ƴϰ�, null ī��Ʈ�� 0���� ũ��
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount; // Dot�� �Ʒ��� �̵�
                    allDots[i, j] = null; // �̵� ���� ��ġ�� null�� ����
                }
            }
            nullCount = 0; // ���� ���� �Ѿ �� null ī��Ʈ �ʱ�ȭ
        }
        yield return new WaitForSeconds(.4f); // 0.4�� ���
        StartCoroutine(FillBoardCo()); // ���带 �ٽ� ä��� �ڷ�ƾ ����
    }


    // ���带 �ٽ� ä��� �Լ�
    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null) // ���� Dot�� null�̶��
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet); // ���ο� Dot�� ��ġ ���
                    int dotToUse = Random.Range(0, dots.Length); // ����� Dot�� �ε��� ������ ����
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity); // Dot ����
                    allDots[i, j] = piece; // ������ Dot�� �迭�� ����
                    piece.GetComponent<Dot>().row = j; // Dot�� �� ����
                    piece.GetComponent<Dot>().column = i; // Dot�� �� ����
                }
            }
        }
    }

    // ���忡 ��ġ�� �ִ��� Ȯ���ϴ� �Լ�
    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null && allDots[i, j].GetComponent<Dot>().isMatched) // ��ġ�� Dot�� �ִ��� Ȯ��
                {
                    return true; // ��ġ�� �ִٸ� true ��ȯ
                }
            }
        }
        return false; // ��ġ�� ���ٸ� false ��ȯ
    }


    // ���带 �ٽ� ä��� ��ġ�� Ȯ���ϴ� �ڷ�ƾ
    private IEnumerator FillBoardCo()
    {
        RefillBoard(); // ���带 �ٽ� ä��
        yield return new WaitForSeconds(.5f); // �ణ�� �����̸� �� ��

        // ���� ���� ��ġ���� ó���ϴ� ���� �ݺ�
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f); // ��ġ�� ó���Ǵ� ���� ��ٸ�
            DestroyMatches(); // ��ġ�� Dot���� �ı�
        }
        findMatches.currentMatches.Clear(); // ���� ��ġ ����� Ŭ����
        currentDot = null; // ���� ���õ� Dot�� ����
        yield return new WaitForSeconds(.5f); // �߰����� ������ ��
        currentState = GameState.move; // ���� ���¸� '������'���� ����
    }
}
