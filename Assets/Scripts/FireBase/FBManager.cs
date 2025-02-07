using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using System.Threading.Tasks;
using System;

public class FBManager : MonoBehaviour
{
    static FBManager _uniqInstance;

    [SerializeField] TMP_InputField idField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField upIDField;
    [SerializeField] TMP_InputField upPSField;
    [SerializeField] GameObject signUpWindow;
    [SerializeField] GameObject nameWindow;
    FirebaseAuth auth;
    FirebaseUser user;
    DatabaseReference dbReference; // TODO: 에러로 인한 주석처리 > FirebaseDatabase SDK 설치 해줘야함
    string LobbyScene = "02_Lobby";

    public static FBManager _instance
    {
        get { return _uniqInstance; }
    }

    void Awake()
    {
        _uniqInstance = this;
        DontDestroyOnLoad(_uniqInstance);
        auth = FirebaseAuth.DefaultInstance;        // 로그인 인증을 관리할 객체를 먼저 선언
        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference; // TODO: 에러로 인한 주석처리 > FirebaseDatabase SDK 설치 해줘야함
                Debug.Log("Firebase 초기화 완료!");
            }
            else
            {
                Debug.LogError("Firebase 초기화 실패: " + task.Result);
            }
        });
        user = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    public void OnClickEmailSignInButton()       // 이메일 로그인
    {
        // 기존 사용자 로그인 함수
        auth.SignInWithEmailAndPasswordAsync(idField.text, passwordField.text).ContinueWithOnMainThread(task =>
        {
            Debug.Log($"Task 상태: {task.Status}"); // Task 상태 출력 (e.g., RanToCompletion, Faulted 등)

            if (task.IsCanceled)
            {
                Debug.Log("Task가 취소되었습니다.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("로그인 실패: " + task.Exception); // 예외 정보 출력
                foreach (var innerException in task.Exception.InnerExceptions)
                {
                    Debug.LogError($"내부 예외: {innerException.Message}"); // 각 내부 예외 출력
                }
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log("Task가 성공적으로 완료되었습니다.");
                SceneManager.LoadScene(LobbyScene);
            }
        });
    }
    public void SignUpWinOpen()
    {
        signUpWindow.SetActive(true);
    }
    public void OnClickSignUpButton()       // 이메일 회원가입
    {
        // 신규 회원가입 함수
        auth.CreateUserWithEmailAndPasswordAsync(upIDField.text, upPSField.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("회원가입 실패");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("회원가입 실패");
                return;
            }
            if (task.IsCompleted)
            {
                Debug.Log("회원가입 성공");
                signUpWindow.SetActive(false);
                nameWindow.SetActive(true);
                return;
            }
        });
    }
    public void SvaeUserInfo()      // 회원가입시 파이어베이스에 유저데이터 정보저장
    {
        if (user == null)
        {
            Debug.LogError("로그인된 사용자가 없음 데이터 저장할 수 없습니다.");
            return;
        }

        string name;

        if (string.IsNullOrEmpty(nameField.text))
        {
            string[] nameDefault = user.Email.Split('@');
            name = nameDefault[0];
            // TODO: 에러로 인한 주석처리 > FirebaseDatabase SDK 설치 해줘야함
            // 닉네임을 안적고 회원가입을 하면 이메일을 닉네임으로 함
            dbReference.Child("Users").Child(user.UserId).Child("Name").SetValueAsync(name).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("유저정보 저장 성공 "+ name);
                    SceneManager.LoadScene(LobbyScene);
                }
                else
                {
                    Debug.LogError("유저정보 저장 실패: " + task.Exception);
                }
            });
        }
        else
        {
            // TODO: 에러로 인한 주석처리 > FirebaseDatabase SDK 설치 해줘야함
            dbReference.Child("Users").Child(user.UserId).Child("Name").SetValueAsync(nameField.text).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("유저정보 저장 성공 "+ nameField.text);
                    SceneManager.LoadScene(LobbyScene);
                }
                else
                {
                    Debug.LogError("유저정보 저장 실패: " + task.Exception);
                }
            });
        }
    }

    public void UserLogout()        // 로그아웃 함수
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    int testWincount = 0;

    public void UserInfoLoad(string name, int wins, int losses, int coin, int icon, int outline)
    {
        if (user == null)
        {
            Debug.LogError("로그인된 사용자가 없습니다!");
            return;
        }
        Debug.Log("유저인포로드함수 진입");

        dbReference.Child("Users").Child(user.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("데이타베이스 접근");
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // 전적 불러오기
                if (snapshot.Child("Record").Exists)
                {
                    object pwin = snapshot.Child("Record").Child("Wins").Value;
                    int wins = Convert.ToInt32(pwin);
                    testWincount = wins;
                    Debug.Log($"Wins 값: {wins}");
                    object plosses = snapshot.Child("Record").Child("Losses").Value;
                    int losses = Convert.ToInt32(plosses);
                    Debug.Log($"Losses 값: {losses}");
                }
                // 게임 전적이 없으면 0 으로 셋팅
                else
                {
                    dbReference.Child("Users").Child(user.UserId).Child("Record").Child("Wins").SetValueAsync(0).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCompleted)
                        {
                            Debug.Log("DB에 승 기록 셋팅");
                        }
                        else
                        {
                            Debug.LogError("기록 셋팅 실패 : " + task.Exception);
                        }
                    });

                    dbReference.Child("Users").Child(user.UserId).Child("Record").Child("Losses").SetValueAsync(0).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCompleted)
                        {
                            Debug.Log("DB에 패 기록 셋팅");
                        }
                        else
                        {
                            Debug.LogError("기록 셋팅 실패 " + task.Exception);
                        }
                    });
                }
                // 코인 불러오기
                if (snapshot.Child("Coin").Exists)
                {
                    object plosses = snapshot.Child("Coin").Value;
                    int losses = Convert.ToInt32(plosses);
                    Debug.Log($"Coin 값: {losses}");
                }
                // 처음 들어왔으면 코인이 없을테니 0으로 셋팅
                else
                {
                    CoinSet();
                }
                if (snapshot.Child("Decoration").Exists)
                {
                    object picon = snapshot.Child("Decoration").Child("icon").Value;
                    int icon = Convert.ToInt32(picon);
                    Debug.Log($"icon 값: {icon}");
                    object poutline = snapshot.Child("Decoration").Child("outline").Value;
                    int outline = Convert.ToInt32(poutline);
                    Debug.Log($"outline 값: {outline}");
                }
                else
                {
                    // 아이콘 기본값으로 설정
                    dbReference.Child("Users").Child(user.UserId).Child("Decoration").Child("icon").SetValueAsync(0).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCompleted)
                        {
                            Debug.Log("치장 옵션 셋팅");
                        }
                        else
                        {
                            Debug.LogError("치장 옵션 셋팅 실패 : " + task.Exception);
                        }
                    });
                    // 아웃라인 기본값으로 설정
                    dbReference.Child("Users").Child(user.UserId).Child("Decoration").Child("outline").SetValueAsync(0).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCompleted)
                        {
                            Debug.Log("치장 옵션 셋팅");
                        }
                        else
                        {
                            Debug.LogError("치장 옵션 셋팅 실패 : " + task.Exception);
                        }
                    });
                }

                //name = snapshot.Child("Name").Value.ToString();
            }
            else
            {
                Debug.LogError("유저 정보 로드 실패: " + task.Exception);
            }
        });
    }

    void CoinSet()
    {
        dbReference.Child("Users").Child(user.UserId).Child("Coin").SetValueAsync(0).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("코인 셋팅");
            }
            else
            {
                Debug.LogError("코인 셋팅 실패 : " + task.Exception);
            }
        });
    }
}
