using Prism.Commands;
using Prism.Mvvm;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiDesktopClient
{
    public class MainWindowViewModel : BindableBase
    {
        #region Fields
        private SamuraiRepository repository;

        private ObservableCollection<Samurai> samuraiCollection;
        private Samurai selectedSamurai;
        private Samurai currentSamurai;
        private Quote selectedQuote;
        private bool isEditing;
        private bool showQuoteEditor;
        private string samuraiName;
        private string realName;
        private string quoteText;

        private DelegateCommand createNewSamuraiCommand;
        private DelegateCommand deleteSamuraiCommand;

        private DelegateCommand saveSamuraiCommand;
        private DelegateCommand cancelSamuraiCommand;

        private DelegateCommand newQuoteCommand;
        private DelegateCommand editQuoteCommand;
        private DelegateCommand deleteQuoteCommand;
        private DelegateCommand saveQuoteCommand;
        private DelegateCommand cancelQuoteCommand;

        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            //List<Samurai> samuraiList = InitFakes();

            this.repository = new SamuraiRepository();
            var samuraiList = this.repository.SamuraisInMemoryList();

            SamuraiCollection.Clear();
            foreach (var item in samuraiList)
            {
                SamuraiCollection.Add(item);
            }
            this.SelectedSamurai = SamuraiCollection.FirstOrDefault();
        }

        private static List<Samurai> InitFakes()
        {
            return new List<Samurai>
            {
                new Samurai
                {
                    Name = "Super",
                    SecretIdentity = new SecretIdentity { RealName = "Fred"},
                    Quotes = new List<Quote>
                    {
                        new Quote {Text = "This is quote number one" },
                        new Quote {Text = "This is quote number two" },
                        new Quote {Text = "This is quote number Three" },
                    }
                },

                new Samurai
                {
                    Name = "Duper",
                    SecretIdentity = new SecretIdentity { RealName = "Bub"},
                    Quotes = new List<Quote>
                    {
                        new Quote {Text = "I am quoted number one" },
                        new Quote {Text = "I am quoted numbertwo" },
                        new Quote {Text = "I am quoted numberr Three" },
                    }
                },

                new Samurai
                {
                    Name = "Kick Ass",
                    SecretIdentity = new SecretIdentity { RealName = "Wilma"},
                    Quotes = new List<Quote>
                    {
                        new Quote {Text = "Heck yea" },
                        new Quote {Text = "What up brah!" },
                        new Quote {Text = "MoFO" },
                    }
                },
            };
        }
        #endregion

        #region Properties

        public ObservableCollection<Samurai> SamuraiCollection
        {
            get { return this.samuraiCollection ?? (this.samuraiCollection = new ObservableCollection<Samurai>()); }
            set { this.SetProperty(ref this.samuraiCollection, value); }
        }
        
        public Samurai SelectedSamurai
        {
            get { return this.selectedSamurai; }
            set { this.SetProperty(ref this.selectedSamurai, value); }
        }

        public Samurai CurrentSamurai
        {
            get { return this.currentSamurai ?? (this.currentSamurai = new Samurai()); }
            set { this.SetProperty(ref currentSamurai, value); }
        }

        public Quote SelectedQuote
        {
            get { return this.selectedQuote; }
            set { this.SetProperty(ref this.selectedQuote, value); }
        }

        public bool IsEditing
        {
            get { return this.isEditing; }
            set
            {
                if (this.SetProperty(ref this.isEditing, value))
                {
                    CreateNewSamuraiCommand.RaiseCanExecuteChanged();
                    DeleteSamuraiCommand.RaiseCanExecuteChanged();
                    SaveSamuraiCommand.RaiseCanExecuteChanged();
                    CancelSamuraiCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool ShowQuoteEditor
        {
            get { return this.showQuoteEditor; }
            set { this.SetProperty(ref this.showQuoteEditor, value); }
        }

        public string SamuraiName
        {
            get { return this.samuraiName; }
            set { this.SetProperty(ref this.samuraiName, value); }
        }

        public string RealName
        {
            get { return this.realName; }
            set { this.SetProperty(ref this.realName, value); }
        }

        public string QuoteText
        {
            get { return this.quoteText; }
            set { this.SetProperty(ref this.quoteText, value); }
        }

        public DelegateCommand CreateNewSamuraiCommand
        {
            get { return this.createNewSamuraiCommand ?? (this.createNewSamuraiCommand = new DelegateCommand(OnCreateNewSamurai, CanCreate)); }
        }

        public DelegateCommand DeleteSamuraiCommand
        {
            get { return this.deleteSamuraiCommand ?? (this.deleteSamuraiCommand = new DelegateCommand(OnDeleteSamurai, CanCreate)); }
        }

        public DelegateCommand SaveSamuraiCommand
        {
            get { return this.saveSamuraiCommand ?? (this.saveSamuraiCommand = new DelegateCommand(OnSaveSamurai, CanSave)); }
        }

        public DelegateCommand CancelSamuraiCommand
        {
            get { return this.cancelSamuraiCommand ?? (this.cancelSamuraiCommand = new DelegateCommand(OnCancelSamurai, CanSave)); }
        }

        public DelegateCommand NewQuoteCommand
        {
            get { return this.newQuoteCommand ?? (this.newQuoteCommand = new DelegateCommand(OnNewQuote)); }
        }

        public DelegateCommand EditQuoteCommand
        {
            get { return this.editQuoteCommand ?? (this.editQuoteCommand = new DelegateCommand(OnEditQuote)); }
        }

        public DelegateCommand DeleteQuoteCommand
        {
            get { return this.deleteQuoteCommand ?? (this.deleteQuoteCommand = new DelegateCommand(OnDeleteQuote)); }
        }
        
        public DelegateCommand SaveQuoteCommand
        {
            get { return this.saveQuoteCommand ?? (this.saveQuoteCommand = new DelegateCommand(OnSaveQuote)); }
        }

        public DelegateCommand CancelQuoteCommand
        {
            get { return this.cancelQuoteCommand ?? (this.cancelQuoteCommand = new DelegateCommand(OnCancelQuote)); }
        }

        #endregion

        #region Command and Event Handlers
        private bool CanCreate()
        {
            // TODO: Dial into ObservesCanExecute  extension method .... if this is need 
            //          BENEFIT...... won't need to RaiseCanExecuteChanged!!!!! yea
            if (IsEditing) return false;
            return true;
        }

        private void OnCreateNewSamurai()
        {
            // Don't like this ... the repo should not be responsible for creating ...
            
            this.IsEditing = true;
        }

        private void OnDeleteSamurai()
        {
            var test = this.SelectedSamurai;
        }

        private bool CanSave()
        {
            if (!IsEditing) return false;
            return true;
        }

        private void OnSaveSamurai()
        {
            this.CurrentSamurai = new Samurai
            {
                Name = SamuraiName,
                SecretIdentity = new SecretIdentity { RealName =  this.RealName }
            };

            this.repository.SaveSamuraiChanges(this.CurrentSamurai);

            //this.repository.SaveChanges(CurrentSamurai.GetType());
            IsEditing = false;
        }
        
        private void OnCancelSamurai()
        {
            IsEditing = false;
        }

        private void OnNewQuote()
        {
            // TODO: create a flyout ... to grab needed data?
            this.ShowQuoteEditor = true;
        }

        private void OnEditQuote()
        {
            var hasValue = this.SelectedQuote;
        }

        private void OnDeleteQuote()
        {
            var hasValue = this.SelectedQuote;
        }

        private void OnSaveQuote()
        {
            this.ShowQuoteEditor = false;
            // Add the SelectedSamurai
            // ... Save Changes
        }

        private void OnCancelQuote()
        {
            this.ShowQuoteEditor = false;
            // TODO: Clear any properties
        }
        #endregion

        #region Methods
        #endregion
    }
}
