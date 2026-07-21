using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUIResourceNotificationSystem : MonoBehaviour
{
    #region Variables
    private static Queue<GameplayResources.ResourceChangeEventArgs> _queuedChanges;
    [SerializeField] private GUIResourceChangeWidget _widgetBase;
    [Tooltip("For now ann resource icons have to be referenced here")]
    [SerializeField] private Sprite _iconAelionEidos, _iconCredits;
    private List<GUIResourceChangeWidget> _spawnedWidgets;
    private float _nextWidgetTimer;
    #endregion

    #region Mono
    private void Awake()
    {
        _nextWidgetTimer = 0;
        if(SkyforgeLoader.CurrentProfile!= null)
        {
            SkyforgeLoader.CurrentProfile.GameplayResources.ResourceChangedEvent += ResourcesChanged;
        }
        _queuedChanges = new();
        _spawnedWidgets = new();
    }
    private void Update()
    {
        //Each second new widget is spawned if the queue isn't empty
        if (_nextWidgetTimer>0)
        {
            _nextWidgetTimer -= Time.deltaTime;
            if(_nextWidgetTimer<=0 && _queuedChanges.Count>0)
            {
                var newResArgs = _queuedChanges.Dequeue();
                var widget = Instantiate(_widgetBase, this.transform);
                Sprite sprite = null;
                if (newResArgs.ResourceType == GameplayResources.ResourceType.AelionEidos)
                    sprite = _iconAelionEidos;
                else if (newResArgs.ResourceType == GameplayResources.ResourceType.Credits)
                    sprite = _iconCredits;
                widget.SetValues(sprite, newResArgs.Amount);
                widget.OnDestroyed += WidgetDestroyed;
                bool otherWidgetsPresent = false;
                //If there are displayed widgets already, they will be moved up
                foreach(var sWidget in _spawnedWidgets)
                {
                    sWidget.GoUp();
                    otherWidgetsPresent = true;
                }
                //Current widget has to wait until others are moved up
                if (otherWidgetsPresent)
                    StartCoroutine(DelayedWidgetActivation(widget, 0.5f));
                _spawnedWidgets.Add(widget);
                //if the queue isn't empty, new timer will be set
                if (_queuedChanges.Any())
                {
                    _nextWidgetTimer = 1f;
                }
                else
                    _nextWidgetTimer = 0;
            }
        }
    }
    private void OnDestroy()
    {
        if (SkyforgeLoader.CurrentProfile != null)
        {
            SkyforgeLoader.CurrentProfile.GameplayResources.ResourceChangedEvent -= ResourcesChanged;
        }
    }
    #endregion

    #region Methods
    private IEnumerator DelayedWidgetActivation(GUIResourceChangeWidget widget, float time)
    {
        widget.gameObject.SetActive(false);
        yield return new WaitForSeconds(time);
        widget.gameObject.SetActive(true);
    }
    #endregion

    #region EventHandlers
    private void WidgetDestroyed(object sender, EventArgs e)
    {
        if (sender is GUIResourceChangeWidget)
        {
            _spawnedWidgets.Remove(sender as GUIResourceChangeWidget);
            ((GUIResourceChangeWidget)sender).OnDestroyed -= WidgetDestroyed;
        }
    }
    private void ResourcesChanged(object sender, GameplayResources.ResourceChangeEventArgs e)
    {
        //When the player profile is changed, the change itself enter the queue to be displayed on the screen
        if(!_queuedChanges.Contains(e))
            _queuedChanges.Enqueue(e);
        if (_nextWidgetTimer == 0)
            _nextWidgetTimer = 1f;
    }
    #endregion
}
