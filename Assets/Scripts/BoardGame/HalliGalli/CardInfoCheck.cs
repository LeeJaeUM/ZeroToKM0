using UnityEngine;
using Unity.Netcode;

public class CardInfoCheck : NetworkBehaviour
{
    public TextMesh myText;
    public int index;

    // 클라이언트에서 실행: 받은 텍스트를 UI에 반영
    [ClientRpc]
    private void UpdateTextClientRpc(string cardInfo, int targetIndex)
    {
        // 자신의 index와 targetIndex가 일치할 때만 업데이트
      //  if (index == targetIndex)
     //   {
            if (myText != null)
            {
                myText.text = cardInfo; // 텍스트 업데이트
            }
       // }
    }

    // 서버에서 실행: 텍스트를 업데이트하고 조건에 맞는 클라이언트로 전송
    void UpdateText(string cardInfo, int playerNum)
    {
        if (IsServer)
        {
            if (playerNum == index) // playerNum이 자신의 index일 때만 실행
            {
                // 서버에서 바로 텍스트 업데이트
                myText.text = cardInfo;

                // 모든 클라이언트로 전송
                UpdateTextClientRpc(cardInfo, index);
            }
        }
    }

    private void Awake()
    {
        myText = GetComponent<TextMesh>();
        myText.text = " ";
    }

    private void Start()
    {
      //  GameManager.Instance.m_halligalli.OnTopCardChanged += UpdateText;
    }

    private void OnDisable()
    {
      //  GameManager.Instance.m_halligalli.OnTopCardChanged -= UpdateText;
    }

}