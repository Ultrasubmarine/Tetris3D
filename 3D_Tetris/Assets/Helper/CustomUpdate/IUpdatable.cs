namespace CustomUpdate
{
    public enum UpdatingMode
    {
        Simple, 
        Fixed,
        Late,
    }
    
    public interface IUpdating
    {
        void AddUpdatableItem( IUpdatable item, UpdatingMode mode = UpdatingMode.Simple);
        void RemoveUpdateItem( IUpdatable item, UpdatingMode mode = UpdatingMode.Simple);
    }
    
    public interface IUpdatable
    {
        void EnableUpdate();
        void DisableUpdate();
        
        void UpdateMe();
    }
}