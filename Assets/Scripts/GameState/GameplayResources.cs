using System;
using System.Xml.Serialization;

public class GameplayResources
{
    public enum ResourceType { Credits, AelionEidos }
    public class ResourceChangeEventArgs : EventArgs
    {
        public GameplayResources.ResourceType ResourceType;
        public int Amount;
    }
    [XmlIgnore]
    public EventHandler<ResourceChangeEventArgs> ResourceChangedEvent;

    //Each new resoure here has to be constructed exactly the way those belowa are. It has to properly trigger events on change, otherwise the GUI may not work as intended
    private int _credits;
    public int Credits {
        get
        {
            return _credits;
        }
        set
        {
            var ch = value - _credits;
            _credits = value;
            ResourceChangedEvent?.Invoke(this, new ResourceChangeEventArgs() { Amount = ch, ResourceType = ResourceType.Credits });
        } }
    private int _aelionEidoses;
    public int AelionEidoses
    {
        get
        {
            return _aelionEidoses;
        }
        set
        {
            var ch = value - _aelionEidoses;
            _aelionEidoses = value;
            ResourceChangedEvent?.Invoke(this, new ResourceChangeEventArgs() { Amount = ch, ResourceType = ResourceType.AelionEidos });
        }
    }
}
