using Autofac;

namespace LoadStoreImages.ViewModels.Base
{
    public class ViewModelLocator
    {
        IContainer container;
        public ViewModelLocator()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MainViewModel>();
            container = builder.Build();
        }

        public MainViewModel MainViewModel
        {
            get { return this.container.Resolve<MainViewModel>(); }
        }
    }
}
