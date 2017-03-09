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
            MainPage = new ContentPage()
            {
                Content = new StackLayout()
                {
                    Padding = new Thickness(40),
                    Children =
                    {
                        new Label
                        {
                            Text =
                                "This is a simple application designed to track user location every fifteen minutes. You're not required to do anything here :)",
                            FontSize = 20,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.CenterAndExpand
                        }
                    }
                }
            };
        }
    }
}
