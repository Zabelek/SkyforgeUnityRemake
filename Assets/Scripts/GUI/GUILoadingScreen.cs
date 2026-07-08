using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUILoadingScreen : MonoBehaviour
{

    #region Variables
    public static string HintText;
    public static Sprite MapThumbnail;
    private bool _nextSceneLoading, _changeExecuted;
    [SerializeField] private Image _loadingAnimation1, _loadingAnimation2, _background;
    [SerializeField] private TextMeshProUGUI _hintTextbox;
    [SerializeField] private GUISceneBlackFade _black;
    #endregion

    #region Mono
    private void Awake()
    {
        _hintTextbox.text = HintText;
        if(MapThumbnail!=null)
            _background.sprite = MapThumbnail;
        _nextSceneLoading = false;
        _changeExecuted = false;
    }
    private void LateUpdate()
    {
        if (!_nextSceneLoading)
        {
            _ = SceneChangePrepare();
        }
        if(SkyforgeLoader.LoadedSceneReady && !_changeExecuted)
        {
            _ = SceneChangeExecute();
            _changeExecuted = true;
        }
        UpdateLoadingAnimation();
    }
    #endregion

    #region Methods
    private async Task SceneChangePrepare()
    {
        _nextSceneLoading = true;
        if (SkyforgeLoader.CurrentProfile != null)
        {
            SaveManager.SaveProfile(SkyforgeLoader.CurrentProfile);
        }
        await SkyforgeLoader.LoadScene();
    }
    private async Task SceneChangeExecute()
    {
        await _black.StartFadeIn();
        _ = SkyforgeLoader.UnloadLoadingScene();
    }
    private void UpdateLoadingAnimation()
    {
        _loadingAnimation1.transform.rotation = Quaternion.Euler(_loadingAnimation1.transform.rotation.eulerAngles + new Vector3(0, 0, 1.5f));
        _loadingAnimation2.transform.rotation = Quaternion.Euler(_loadingAnimation2.transform.rotation.eulerAngles + new Vector3(0, 0, 1f));
    }
    #endregion
}
