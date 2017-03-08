using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LocationTracker.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            MainPage = new ContentPage() {Content = new Label {Text = "Testing..."}};
        }

        protected override void OnStart()
        {
            base.OnStart();
        }
    }
}
