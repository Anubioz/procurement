using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using POEApi.Model;
using Procurement.ViewModel;
using POEApi.Infrastructure;

namespace Procurement.Controls
{
    public partial class ItemDisplay : UserControl
    {
        private static List<Popup> annoyed = new List<Popup>();

        public ItemDisplay()
        {
            InitializeComponent();
   
            this.Loaded += new RoutedEventHandler(ItemDisplay_Loaded);
        }

        public static void ClosePopups()
        {
            closeOthersButNot(new Popup());
        }

        void ItemDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            ItemDisplayViewModel vm = this.DataContext as ItemDisplayViewModel;
            Image i = vm.getImage();

            UIElement socket = vm.getSocket();

            this.MainGrid.Children.Add(i);

            if (socket != null)
                doSocketOnHover(socket, i);

            this.Height = i.Height;
            this.Width = i.Width;
           
            this.Loaded -= new RoutedEventHandler(ItemDisplay_Loaded);

        }


        private void doSocketAlwaysOver(UIElement socket)
        {
            this.MainGrid.Children.Add(socket);
        }

        private void doSocketOnHover(UIElement socket, Image i)
        {
            NonTopMostPopup popup = new NonTopMostPopup();
            popup.PopupAnimation = PopupAnimation.Fade;
            popup.StaysOpen = true;
            popup.Child = socket;
            popup.Placement = PlacementMode.Center;
            popup.PlacementTarget = i;
            popup.AllowsTransparency = true;
            i.MouseEnter += (o, ev) =>
            {
                closeOthersButNot(popup);
                popup.IsOpen = true;
            };

            i.MouseLeave += (o, ev) =>
            {
                Rect rect = System.Windows.Media.VisualTreeHelper.GetDescendantBounds(i);
                if (!rect.Contains(ev.GetPosition(o as IInputElement)))
                    popup.IsOpen = false;
            };

            this.MainGrid.Children.Add(popup);
            annoyed.Add(popup);
        }


        void buyoutControl_RemoveClicked(string amount, string orbType)
        {
            
        }

        void buyoutView_SaveClicked(string amount, string orbType)
        {
           
        }

        public static void closeOthersButNot(Popup current)
        {       }
    }
}
