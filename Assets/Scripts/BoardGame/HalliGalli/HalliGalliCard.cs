using Unity.Netcode;
using UnityEngine;

public class HalliGalliCard : Card
{
    // 위치와 회전을 동기화할 네트워크 변수
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(new Vector3(0, 0, 0));
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity);

    void Update()
    {
        // 서버에서만 위치와 회전 업데이트 (클라이언트는 읽기만 함)
        if (IsServer)
        {
            // 위치와 회전 값 갱신
            networkPosition.Value = transform.position;
            networkRotation.Value = transform.rotation;
        }
    }

    // 위치와 회전이 변경되었을 때 클라이언트에서 처리
    private void OnEnable()
    {
        networkPosition.OnValueChanged += HandlePositionChanged;
        networkRotation.OnValueChanged += HandleRotationChanged;
    }

    private void OnDisable()
    {
        networkPosition.OnValueChanged -= HandlePositionChanged;
        networkRotation.OnValueChanged -= HandleRotationChanged;
    }

    private void HandlePositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        // 위치가 변경되면 클라이언트에서 해당 위치로 이동
        transform.position = newPosition;
    }

    private void HandleRotationChanged(Quaternion oldRotation, Quaternion newRotation)
    {
        // 회전이 변경되면 클라이언트에서 해당 회전으로 회전
        transform.rotation = newRotation;
    }

    public enum AnimalType
    {
        Corc,
        Lion,
        Fox,
        Panda
    }
    public AnimalType m_AnimalType;       // 과일 종류
    public int m_fruitNum;              // 과일 개수

    public void Initialize(AnimalType type, int num)
    {
        m_AnimalType = type;
        m_fruitNum = num;
    }
}
