namespace Mantis.Domain.Shared.Services
{
    public class SidebarService
    {
        public event Action OnMouseEnter;

        public void TriggerMouseEnter()
        {
            OnMouseEnter?.Invoke();
        }
    }
}
