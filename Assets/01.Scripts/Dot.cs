using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    // Dot의 열(column)과 행(row) 위치를 저장하는 변수들
    public int column;
    public int row;

    // 이동 목표 위치
    public int targetX;
    public int targetY;

    // 이동 전 위치 기억
    public int previousColumn;
    public int previousRow;

    // 매치 여부
    public bool isMatched = false;

    // 매치 확인에 대한 참조
    private FindMatches findMatches;

    // 게임 보드에 대한 참조
    private Board board;

    // 이 Dot과 교환할 다른 Dot을 저장하는 변수
    public GameObject otherDot;

    // 터치 입력 관련 변수
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    // 스와이프 각도
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    // 폭탄 관련 변수
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;

    // Start is called before the first frame update
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        // 게임 보드 참조 설정
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();

        // 초기 위치 설정
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            isColorBomb = true;
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //FindMatches();
        
        /*
        if(isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f); 
        }
        */
        // 이동 목표 위치 업데이트
        targetX = column;
        targetY = row;

        // X 축 이동 처리
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if(board.allDots[column,row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        // Y 축 이동 처리
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    // 매치가 되었는지 확인하는 함수 (매치가 안되었을 시 기존 자리로 되돌아간다)
    public IEnumerator CheckMoveCo()
    {
        if(isColorBomb)
        {
            findMatches.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        }
        else if(otherDot.GetComponent<Dot>().isColorBomb)
        {
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }
        yield return new WaitForSeconds(.5f);
        if(otherDot != null)
        {
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            //otherDot = null;
        }
    }

    // 마우스가 눌렸을 때 호출되는 메서드
    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    // 마우스에서 손을 땔 때 호출되는 메서드
    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 스와이프 각도 계산 및 처리
            CalculateAngle();
        }
    }

    // 스와이프 각도를 계산하고 이동 처리 메서드를 호출하는 메서드
    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
            swipeAngle = (swipeAngle + 360) % 360; // 각도 범위를 0에서 360도로 매핑
            MovePieces();
            board.currentState = GameState.wait;
            board.currentDot = this;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    // Dot을 이동시키는 메서드
    private void MovePieces()
    {
        // 오른쪽 이동 
        if ((swipeAngle > 315 || swipeAngle <= 45) && column < board.width - 1)
        {
            otherDot = board.allDots[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        // 왼쪽 이동
        else if (swipeAngle > 135 && swipeAngle <= 225 && column > 0)
        {
            otherDot = board.allDots[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        // 위로 이동
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            otherDot = board.allDots[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        // 아래로 이동
        else if (swipeAngle > 225 && swipeAngle <= 315 && row > 0)
        {
            otherDot = board.allDots[column, row - 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }

    // 타일의 매치 여부를 확인하는 함수
    private void FindMatches()
    {
        // 좌 우 매치 여부 확인
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
        // 상 하 매치 여부 확인 
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

    // 폭탄 생성 하는 부분 
    // 세로 폭탄 생성
    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
        findMatches.currentMatches.Clear();
        isMatched = false;
    }

    // 가로 폭탄 생성 
    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
        findMatches.currentMatches.Clear();
        isMatched = false;
    }
}