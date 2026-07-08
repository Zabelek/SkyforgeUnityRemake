using System.Collections.Generic;

public class Damage
{
    #region Variables
    public CharacterBehaviour Source { get; set; }
    public int Amount { get; set; }
    public bool Critical { get; set; }
    public bool Range { get; set; }
    //for multishot damage type
    public List<int> PreviousAmounts { get; set; }
    #endregion

    #region Constructors
    public Damage(CharacterBehaviour source, int amount)
    {
        Source = source;
        Amount = amount;
        Critical = false;
        Range = false;
        PreviousAmounts = new();
    }
    public Damage(CharacterBehaviour source, int amount, bool critical)
    {
        Source = source;
        Amount = amount;
        Critical = critical;
        Range = false;
        PreviousAmounts = new();
    }
    public Damage(CharacterBehaviour source, int amount, bool critical, bool range)
    {
        Source = source;
        Amount = amount;
        Critical = critical;
        Range = range;
        PreviousAmounts = new();
    }
    #endregion

    #region Methods
    public void AddMultishot(Damage damage)
    {
        //abilities that deal damage multiple times usually treat all these damages as one
        PreviousAmounts.Add(this.Amount);
        this.Amount = damage.Amount;
        if (!Critical && damage.Critical)
            Critical = true;
    }
    #endregion
}
