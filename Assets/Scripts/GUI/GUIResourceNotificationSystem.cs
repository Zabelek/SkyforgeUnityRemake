using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUIResourceNotificationSystem : MonoBehaviour
{
    public class ResourceChangeEventArgs : EventArgs
    {
        public string ResourceType;
        public int Amount;
    }
    #region Variables
    [SerializeField] private GUIResourceChangeWidget _widgetBase;
    [SerializeField] private Sprite _iconAelionEidos, _iconCredits;
    private List<GUIResourceChangeWidget> _spawnedWidgets;
    private Queue<ResourceChangeEventArgs> _queuedChanges;
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
        if(_nextWidgetTimer>0)
        {
            _nextWidgetTimer -= Time.deltaTime;
            if(_nextWidgetTimer<=0)
            {
                var newResArgs = _queuedChanges.Dequeue();
                var widget = Instantiate(_widgetBase, this.transform);
                Sprite sprite = null;
                if (newResArgs.ResourceType == "AelionEidoses")
                    sprite = _iconAelionEidos;
                else if (newResArgs.ResourceType == "Credits")
                    sprite = _iconCredits;
                widget.SetValues(sprite, newResArgs.Amount);
                widget.OnDestroyed += WidgetDestroyed;
                bool otherWidgetsPresent = false;
                foreach(var sWidget in _spawnedWidgets)
                {
                    sWidget.GoUp();
                    otherWidgetsPresent = true;
                }
                if(otherWidgetsPresent)
                    StartCoroutine(DelayedWidgetActivation(widget, 0.5f));
                _spawnedWidgets.Add(widget);
                if (_queuedChanges.Any())
                {
                    _nextWidgetTimer = 1f;
                }
                else
                    _nextWidgetTimer = 0;
            }
        }
    }

    private IEnumerator DelayedWidgetActivation(GUIResourceChangeWidget widget, float time)
    {
        widget.gameObject.SetActive(false);
        yield return new WaitForSeconds(time);
        widget.gameObject.SetActive(true);
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
    #endregion

    #region EventHandlers
    private void WidgetDestroyed(object sender, EventArgs e)
    {
        if (sender is GUIResourceChangeWidget)
            _spawnedWidgets.Remove(sender as GUIResourceChangeWidget);
    }

    private void ResourcesChanged(object sender, ResourceChangeEventArgs e)
    {
        _queuedChanges.Enqueue(e);
        if (_nextWidgetTimer == 0)
            _nextWidgetTimer = 1f;
    }
    #endregion
}
