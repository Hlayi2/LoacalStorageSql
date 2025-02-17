using Storage1.Views;

namespace Storage1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ShoppingCartPage), typeof(ShoppingCartPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));

        }
    }
}
