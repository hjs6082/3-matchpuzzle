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

    // 게임 보드에 대한 참조
    private Board board;

    // 이 Dot과 교환할 다른 Dot을 저장하는 변수
    private GameObject otherDot;

    // 터치 입력 관련 변수
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    // 스와이프 각도
    public float swipeAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        // 게임 보드 참조 설정
        board = FindObjectOfType<Board>();

        // 초기 위치 설정
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
    }

    // Update is called once per frame
    void Update()
    {
        // 이동 목표 위치 업데이트
        targetX = column;
        targetY = row;

        // X 축 이동 처리
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            // 보드에 현재 Dot 위치 갱신
            board.allDots[column, row] = this.gameObject;
        }

        // Y 축 이동 처리
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            // 보드에 현재 Dot 위치 갱신
            board.allDots[column, row] = this.gameObject;
        }
    }

    // 마우스가 눌렸을 때 호출되는 메서드
    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // 마우스에서 손을 땔 때 호출되는 메서드
    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 스와이프 각도 계산 및 처리
        CalculateAngle();
    }

    // 스와이프 각도를 계산하고 이동 처리 메서드를 호출하는 메서드
    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
        swipeAngle = (swipeAngle + 360) % 360; // 각도 범위를 0에서 360도로 매핑
        MovePieces();
    }

    // Dot을 이동시키는 메서드
    private void MovePieces()
    {
        // 오른쪽 이동 
        if ((swipeAngle > 315 || swipeAngle <= 45) && column < board.width - 1)
        {
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        // 왼쪽 이동
        else if (swipeAngle > 135 && swipeAngle <= 225 && column > 0)
        {
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        // 위로 이동
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        // 아래로 이동
        else if (swipeAngle > 225 && swipeAngle <= 315 && row > 0)
        {
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
    }
}