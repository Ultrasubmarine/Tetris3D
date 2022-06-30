namespace Script.Influence
{
    public interface IInfluence
    {
        bool Update(float speed = 1);

        void UnlinkCallback();

        bool IsIgnoreSlow();
        
    }
}