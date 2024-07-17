namespace ReitsKit.UI.Interface
{
    public interface ICloneableUI
    {
        public T CLone<T>(T uie) where T : UIElement;
    }
}