using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLoadingManager : MonoBehaviour
{
    public string Token;
    public string Error;
    public string id;
    [SerializeField] TMPro.TMP_Text TMP_Text;
    // Start is called before the first frame update
    void Start()
    {
        Analytics.Start();
        Application.targetFrameRate = 100;

        var a = PlayGamesPlatform.Activate();
        LoginGooglePlayGames();

    }
    public void LoginGooglePlayGames()
    {
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play games successful.");

                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Debug.Log("Authorization code: " + code);
                    Token = code;

                    id = code;
                    TMP_Text.text = id;
                    AuthenticationService.Instance.SignInWithGoogleAsync(id).Start();
                    // This token serves as an example to be used for SignInWithGooglePlayGames
                });
                SceneManager.LoadSceneAsync(1);
            }
            else
            {
                Error = "Failed to retrieve Google play games authorization code";
                PlayGamesPlatform.Instance.RequestServerSideAccess(false, token =>
                {
                    id = token;
                    TMP_Text.text = id;
                }
                );
                Debug.Log("Login Unsuccessful");
            }
        });
    }
}
