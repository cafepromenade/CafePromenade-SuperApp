using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CafePromenade.Desktop.Views;

namespace CafePromenade.Desktop;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        Title = "CafePromenade SuperApp";
        NavView.SelectedItem = NavView.MenuItems[0];
        ContentFrame.Navigate(typeof(DashboardPage));
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            Type? pageType = tag switch
            {
                "Dashboard" => typeof(DashboardPage),
                "Repositories" => typeof(RepositoriesPage),
                "VeraCrypt" => typeof(VeraCryptPage),
                "NTLite" => typeof(NtlitePage),
                "Messaging" => typeof(MessagingPage),
                "PowerToys" => typeof(PowerToysPage),
                "Docker" => typeof(DockerPage),
                "ThreeDPrint" => typeof(ThreeDPrintPage),
                "Minecraft" => typeof(MinecraftPage),
                "Downloads" => typeof(DownloadsPage),
                "SystemTools" => typeof(SystemToolsPage),
                "Automation" => typeof(AutomationPage),
                "About" => typeof(AboutPage),
                _ => typeof(DashboardPage)
            };
            if (pageType != null && ContentFrame.CurrentSourcePageType != pageType)
                ContentFrame.Navigate(pageType);
        }
    }
}
