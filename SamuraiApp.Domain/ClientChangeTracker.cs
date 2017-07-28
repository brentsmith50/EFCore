using Prism.Mvvm;

namespace SamuraiApp.Domain
{
    public class ClientChangeTracker : BindableBase
    {
        private bool isDirty;

        public bool IsDirty
        {
            get { return this.isDirty; }
            set { this.SetProperty(ref this.isDirty, value); }
        }
    }
}
