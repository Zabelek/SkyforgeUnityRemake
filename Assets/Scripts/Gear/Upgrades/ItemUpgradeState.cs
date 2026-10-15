public class ItemUpgradeState
{
    #region Variables
    public float[] EtherFibers;
    public enum EtherDirection
    {
        King, KingHalf, Assassin, AssassinHalf, Sage, SageHalf, Judge, JudgeHalf
    }
    public EtherDirection PickedEtherDirection;
    public short CurrentActiveFiber;
    public short Generation;
    public long CurrentExp;
    #endregion

    #region Constructors
    public ItemUpgradeState()
    {
        EtherFibers = new float[20];
        for(int i=0; i<20; i++)
        {
            EtherFibers[i] = 0;
        }
        CurrentActiveFiber = 0;
        CurrentExp = 0;
        Generation = 1;
    }
    #endregion

    #region Methods
    public long GetMaxExp()
    {
        short currentGen = 1;
        long ret = 100000;
        while(currentGen < Generation)
        {
            ret = (long)(ret*1.25f);
            currentGen++;
        }
        return ret;
    }
    public void AddExp(long exp)
    {
        CurrentExp += exp;
        var maxExp = GetMaxExp();
        if (CurrentExp > maxExp)
        {
            CurrentExp = maxExp;
        }
    }
    public void GenerationUp()
    {
        CurrentExp = 0;
        Generation++;
    }
    internal bool CanGenUp()
    {
        return CurrentExp == GetMaxExp();
    }
    #endregion
}
