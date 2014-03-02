using System.Collections.Generic;
using System.Text;
using POEApi.Model;
using Procurement.ViewModel.Filters;
using System.Linq;
using POEApi.Infrastructure;

namespace Procurement.ViewModel.ForumExportVisitors
{
    internal abstract class VisitorBase : IVisitor
    {
        private const string LINKITEM = "[linkItem location=\"{0}\" league=\"{1}\" x=\"{2}\" y=\"{3}\"]";

        protected virtual bool buyoutItemsOnlyVisibleInBuyoutsTag
        {
            get { return Settings.UserSettings.GetEntry("BuyoutItemsOnlyVisibleInBuyoutsTag").ToLower() == "true"; }
        }

        protected virtual bool embedBuyouts
        {
            get { return Settings.UserSettings.GetEntry("EmbedBuyouts").ToLower() == "true"; }
        }

        public abstract string Visit(IEnumerable<Item> items, string current);
        
        protected string runFilter<T>(IEnumerable<Item> items) where T : IFilter, new()
        {
            return runFilter(new T(), items);
        }

        protected string runFilter(IFilter filter, IEnumerable<Item> items)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in items.Where(i => filter.Applicable(i)))
                builder.Append(getLinkItem(item));

            return builder.ToString();
        }

        protected string getLinkItem<T>(T item) where T : Item
        {
            bool isBuyoutItem = Settings.Buyouts.ContainsKey(item.UniqueIDHash);
            if (isBuyoutItem && buyoutItemsOnlyVisibleInBuyoutsTag)
                return string.Empty;

            if (isBuyoutItem && embedBuyouts)
                return string.Format("\n[linkItem location=\"{0}\" league=\"{1}\" x=\"{2}\" y=\"{3}\"]\n~b/o {4}\n", item.inventoryId, ApplicationState.CurrentLeague, item.X, item.Y, Settings.Buyouts[item.UniqueIDHash]);

            return string.Format("[linkItem location=\"{0}\" league=\"{1}\" x=\"{2}\" y=\"{3}\"]", item.inventoryId, ApplicationState.CurrentLeague, item.X, item.Y);
        }
    }
}
