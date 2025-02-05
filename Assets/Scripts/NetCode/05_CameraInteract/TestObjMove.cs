using System.Collections;
using UnityEngine;

public class TestObjMove : MonoBehaviour
{
    public Vector3 testVec = new Vector3(2, 0, 0);
    public Vector3 testVecMi = new Vector3(-2, 0, 0);
    public float moveDuration = 0.2f;
    private void Start()
    {
        
    }

    private void Update()
    {
        // 숫자 키 1을 누르면 실행
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("숫자 1을 눌렀습니다! 동작을 실행합니다.");
            StartCoroutine(SmoothMoveToPosition(transform.position + testVec)); // 1번 동작
        }

        // 숫자 키 2를 누르면 실행
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("숫자 2를 눌렀습니다! 다른 동작을 실행합니다.");
            StartCoroutine(SmoothMoveToPosition(transform.position + testVecMi)); // 2번 동작
        }
    }

    //private IEnumerator SmoothMoveToPosition(Vector3 targetPosition)
    //{
    //    // 시작 위치와 시간을 기록
    //    Vector3 startPosition = transform.position;
    //    float elapsedTime = 0f;

    //    // moveDuration 동안 부드럽게 이동
    //    while (elapsedTime < moveDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsedTime / moveDuration); // 0~1 사이의 비율 계산
    //        transform.position = Vector3.Lerp(startPosition, targetPosition, t); // 보간

    //        yield return null; // 한 프레임 대기
    //    }

    //    // 목표 위치에 정확히 도달
    //    transform.position = targetPosition;
    //}

    private IEnumerator SmoothMoveToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            t = Mathf.SmoothStep(0, 1, t); // 부드러운 곡선 비율 계산 (ease-in/out)

            transform.position = Vector3.Lerp(startPosition, targetPosition, t); // 보간

            yield return null; // 한 프레임 대기
        }

        // 목표 위치에 정확히 도달
        transform.position = targetPosition;
    }

}
