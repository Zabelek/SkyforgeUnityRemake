using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIResourceChangeGameMenuBottomWidget : MonoBehaviour
{
    #region Variables
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;
    private float _spawnTimer;
    #endregion

    #region Mono
    private void Awake()
    {
        _spawnTimer = 0;
    }
    private void Update()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer < 0.5f)
        {
            //sliding up
        }
        else if (_spawnTimer < 5)
        {
            //nothing
        }
        else if (_spawnTimer < 6)
        {
            //fading out;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Methods
    public void SetValues(string text, Sprite sprite)
    {

    }
    #endregion
}
