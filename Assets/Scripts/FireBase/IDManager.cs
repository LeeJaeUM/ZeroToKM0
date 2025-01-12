using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Google;
using TMPro;

public class IDManager : MonoBehaviour
{
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;
    FirebaseAuth firebaseAuth;
    FirebaseUser firebaseUser;


    void Awake()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;        // 로그인 인증을 관리할 객체를 먼저 선언       
    }

    public void OnClickSignUpButton()       // 이메일 회원가입
    {
        // 신규 회원가입 함수
        firebaseAuth.CreateUserWithEmailAndPasswordAsync(emailField.text, passwordField.text).ContinueWith(task =>
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
                return;
            }
        });

    }

    public void OnClickEmailSignInButton()       // 이메일 로그인
    {
        // 기존 사용자 로그인 함수
        firebaseAuth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text).ContinueWithOnMainThread(task =>
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
                firebaseUser = task.Result.User;
                Debug.Log("로그인 성공: " + firebaseUser);
                Debug.Log($"UID: {firebaseUser.UserId}"); // 고유 ID
                Debug.Log($"이메일: {firebaseUser.Email}"); // 이메일
                Debug.Log("이메일 로그인 성공");
                SceneManager.LoadScene("02_Lobby");
            }
        });
    }

    public void OnClickGoogleSignInButton()     // 구글 로그인
    {
        //Credential credential = GoogleAuthProvider.GetCredential(emailField.text, null);
        //firebaseAuth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
        //    if (task.IsCanceled)
        //    {
        //        Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
        //        return;
        //    }
        //    if (task.IsFaulted)
        //    {
        //        Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
        //        return;
        //    }
        //    if (task.IsCompleted)
        //    {
        //        Debug.Log("로그인 성공 :" + firebaseUser);
        //        SceneManager.LoadScene(1);
        //    }

        //    AuthResult result = task.Result;
        //    Debug.LogFormat("User signed in successfully: {0} ({1})",
        //        result.User.DisplayName, result.User.UserId);
        //});


        // Google Sign-In 시도
        //GoogleSignIn.Configuration = configuration;
        //GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
        //{
        //    if (task.IsCanceled)
        //    {
        //        Debug.LogError("Google Sign-In was canceled.");
        //        return;
        //    }
        //    if (task.IsFaulted)
        //    {
        //        Debug.LogError("Google Sign-In failed: " + task.Exception);
        //        return;
        //    }

        //    // Google 로그인 성공 -> ID 토큰 가져오기
        //    GoogleSignInUser googleUser = task.Result;
        //    string idToken = googleUser.IdToken;

        //    // Firebase 인증 처리
        //    Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        //    firebaseAuth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
        //    {
        //        if (authTask.IsCanceled)
        //        {
        //            Debug.LogError("Firebase Sign-In was canceled.");
        //            return;
        //        }
        //        if (authTask.IsFaulted)
        //        {
        //            Debug.LogError("Firebase Sign-In failed: " + authTask.Exception);
        //            return;
        //        }

        //        // Firebase 로그인 성공
        //        FirebaseUser firebaseUser = authTask.Result;
        //        Debug.LogFormat("Firebase Sign-In successful: {0} ({1})", firebaseUser.DisplayName, firebaseUser.UserId);

        //        // 로그인 성공 후 다음 씬으로 이동
        //        SceneManager.LoadScene(1);
        //    });
        //});
    }
}
