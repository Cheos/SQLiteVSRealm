using SQLiteVsRealm.ViewModels;
using Xamarin.Forms;

namespace SQLiteVsRealm.Views
{
    public partial class SQLiteVsRealmPage : ContentPage
    {
        public SQLiteVsRealmPage()
        {
            InitializeComponent();

            var viewModel = new SQLiteVsRealmViewModel();
            viewModel.Init();
            this.BindingContext = viewModel;
        }
    }
}

