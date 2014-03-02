using System.Linq;
using System.Windows.Controls;

namespace Procurement.Controls
{
    public partial class LeagueSelection : UserControl
    {
        public LeagueSelection()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            string league = cb.SelectedItem.ToString();
            ApplicationState.CurrentLeague = league;
            ApplicationState.CurrentCharacter = ApplicationState.Characters.First(c => c.League == ApplicationState.CurrentLeague);
        }
    }
}
