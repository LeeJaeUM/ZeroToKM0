using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class TestNetworkVariable : NetworkBehaviour
{
    [SerializeField]
    NetworkVariable<bool> olVar;
    [SerializeField] NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

    public BoxCollider boxCollider;
    public float speed = 10;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>(); 
        
        if (!IsOwner)
        {
            Position.OnValueChanged += OnPositionChanged; // 값이 변경될 때마다 호출
        }
    }    
    // Position이 변경되었을 때 클라이언트에서 처리하는 함수
    private void OnPositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        // 서버나 다른 클라이언트에서 Position이 변경되었을 때 호출
        transform.position = newPosition;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestAnimationServerRpc();
        }
        //Move();
        ActionMove();
    }

    [ServerRpc]
    void TestAnimationServerRpc()
    {
        olVar.Value = !olVar.Value;
        animator.SetTrigger("Test");
    }    
    


    // 위치를 서버로 전송하고 서버에서 Position을 업데이트
    [ServerRpc]
    void SubmitPositionRequestServerRpc(Vector3 movedPosition)
    {
        Debug.Log("ServerRpc 호출");
        Position.Value += movedPosition; // 서버에서 Position.Value를 갱신
    }



    // 플레이어 이동
    void ActionMove()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        if (moveHorizontal != 0)
        {
            // 이동
            Vector3 movement = new Vector3(moveHorizontal, 0f, 0f) * speed * Time.deltaTime;
            transform.position += movement;
            SubmitPositionRequestServerRpc(movement); // 서버에 위치 업데이트 요청
        }
    }

    //void Move()
    //{
    //    float moveHorizontal = Input.GetAxis("Horizontal");

    //    if (moveHorizontal != 0)
    //    {
    //        // 플레이어가 움직이는 방향에 따라 회전
    //        if (moveHorizontal < 0)  // 왼쪽으로 이동
    //        {
    //            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
    //        }
    //        else if (moveHorizontal > 0)  // 오른쪽으로 이동
    //        {
    //            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    //        }
    //        // 이동
    //        Vector3 movement = new Vector3(moveHorizontal, 0f, 0f);
    //        transform.position += movement * speed * Time.deltaTime;
    //        SubmitPositionRequestServerRpc(transform.position);
    //    }

    //}

    //[ServerRpc]
    //void SubmitPositionRequestServerRpc(Vector3 movedPosition)
    //{
    //    Debug.Log("서버로 보냈어 ");
    //    transform.position = movedPosition;
    //    Position.Value = movedPosition;
    //    UpdateClientPositionClientRpc(movedPosition);
    //}

    //[ClientRpc]
    //void UpdateClientPositionClientRpc(Vector3 newPosition)
    //{
    //    if (!IsOwner)
    //    {
    //        transform.position = newPosition;
    //    }
    //}
}
