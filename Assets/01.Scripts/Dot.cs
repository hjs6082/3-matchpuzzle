using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    // Dot�� ��(column)�� ��(row) ��ġ�� �����ϴ� ������
    public int column;
    public int row;

    // �̵� ��ǥ ��ġ
    public int targetX;
    public int targetY;

    // �̵� �� ��ġ ���
    public int previousColumn;
    public int previousRow;

    // ��ġ ����
    public bool isMatched = false;

    // ���� ���忡 ���� ����
    private Board board;

    // �� Dot�� ��ȯ�� �ٸ� Dot�� �����ϴ� ����
    private GameObject otherDot;

    // ��ġ �Է� ���� ����
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    // �������� ����
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // ���� ���� ���� ����
        board = FindObjectOfType<Board>();

        // �ʱ� ��ġ ����
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
        previousRow = row;
        previousColumn = column;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();
        if(isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f); 
        }
        // �̵� ��ǥ ��ġ ������Ʈ
        targetX = column;
        targetY = row;

        // X �� �̵� ó��
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            // ���忡 ���� Dot ��ġ ����
            board.allDots[column, row] = this.gameObject;
        }

        // Y �� �̵� ó��
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            // ���忡 ���� Dot ��ġ ����
            board.allDots[column, row] = this.gameObject;
        }
    }

    // ��ġ�� �Ǿ����� Ȯ���ϴ� �Լ� (��ġ�� �ȵǾ��� �� ���� �ڸ��� �ǵ��ư���)
    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherDot != null)
        {
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            otherDot = null;
        }
    }

    // ���콺�� ������ �� ȣ��Ǵ� �޼���
    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // ���콺���� ���� �� �� ȣ��Ǵ� �޼���
    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // �������� ���� ��� �� ó��
        CalculateAngle();
    }

    // �������� ������ ����ϰ� �̵� ó�� �޼��带 ȣ���ϴ� �޼���
    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
            swipeAngle = (swipeAngle + 360) % 360; // ���� ������ 0���� 360���� ����
            MovePieces();
        }
    }

    // Dot�� �̵���Ű�� �޼���
    private void MovePieces()
    {
        // ������ �̵� 
        if ((swipeAngle > 315 || swipeAngle <= 45) && column < board.width - 1)
        {
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        // ���� �̵�
        else if (swipeAngle > 135 && swipeAngle <= 225 && column > 0)
        {
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        // ���� �̵�
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        // �Ʒ��� �̵�
        else if (swipeAngle > 225 && swipeAngle <= 315 && row > 0)
        {
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }

    // Ÿ���� ��ġ ���θ� Ȯ���ϴ� �Լ�
    private void FindMatches()
    {
        // �� �� ��ġ ���� Ȯ��
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag && leftDot1.tag == rightDot1.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        // �� �� ��ġ ���� Ȯ�� 
        if (row > 0 && row < board.height - 1)
        {
            GameObject downDot1 = board.allDots[column, row - 1];
            GameObject upDot1 = board.allDots[column , row + 1];
            if (downDot1 != null && upDot1 != null)
            {
                if (downDot1.tag == this.gameObject.tag && upDot1.tag == this.gameObject.tag && downDot1.tag == upDot1.tag)
                {
                    downDot1.GetComponent<Dot>().isMatched = true;
                    upDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
}