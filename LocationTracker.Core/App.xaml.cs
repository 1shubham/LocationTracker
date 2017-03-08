using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
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
            MobileCenter.Start(typeof(Analytics), typeof(Crashes));
            MainPage = new ContentPage() {Content = new Label {Text = "Testing..."}};
        }

        protected override void OnStart()
        {
            base.OnStart();
        }
    }
}
