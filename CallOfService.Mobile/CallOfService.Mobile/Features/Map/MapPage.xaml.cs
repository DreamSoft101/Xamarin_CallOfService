using CallOfService.Mobile.Messages;
using CallOfService.Mobile.UI;
using PubSub;
using TK.CustomMap;

namespace CallOfService.Mobile.Features.Map
{
    public partial class MapPage : BasePage
    {
        public MapPage()
        {
            InitializeComponent();
        }

        private void Map_OnCalloutClicked(object sender, TKGenericEventArgs<TKCustomMapPin> e)
        {
            if(e.Value?.BindingContext != null)
                this.Publish(new JobSelected((int)e.Value.BindingContext));
        }
    }
}