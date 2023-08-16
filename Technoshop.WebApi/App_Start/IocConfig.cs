using Autofac.Integration.WebApi;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Technoshop.Common;
using Technoshop.Repository;
using Technoshop.Repository.Common;
using Technoshop.Service.Common;
using Technoshop.Service;
using Technoshop.Model;
using Microsoft.Extensions.Logging;

namespace Technoshop.WebApi.App_Start
{
    public class IocConfig
    {
        public static IContainer Container;

        public static void Initialize(HttpConfiguration config)
        {
            Initialize(config, RegisterServices(new ContainerBuilder()));
        }
        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<EnvironmentVariableConnectionStringProvider>().As<IConnectionStringProvider>().SingleInstance();

            //ORDERS
            builder.RegisterType<OrderRepository>().As<IOrderRepository>().InstancePerRequest();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerRequest();
            builder.RegisterType<OrderValidationService>().As<IOrderValidationService>().InstancePerRequest();

            //PRODUCTS
            builder.RegisterType<ProductRepository>().As<IProductRepository>().InstancePerRequest();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerRequest();

            //CATEGORY 
            builder.RegisterType<CategoryRepository>().As<ICategoryRepository>().InstancePerRequest();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerRequest();
           
            //USERS
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();

            //PERSON
            builder.RegisterType<PersonRepository>().As<IPersonRepository>().InstancePerRequest();
            builder.RegisterType<PersonService>().As<IPersonService>().InstancePerRequest(); 
            //ROLE
            builder.RegisterType<RoleRepository>().As<IRoleRepository>().InstancePerRequest();
            builder.RegisterType<RoleService>().As<IRoleService>().InstancePerRequest();

            //ADDRESS
            builder.RegisterType<AddressRepository>().As<IAddressRepository>().InstancePerRequest();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerRequest();

            /*
             * HELPERS
             */

            //mail
            builder.RegisterType <MailerService>().As<IMailerService>().InstancePerDependency();
            //pdf
            builder.RegisterType<PdfService>().As<IPdfService>().InstancePerDependency();
            //paginate
            builder.RegisterType<PaginateService>().As<IPaginateService>().InstancePerDependency();
            //connection
            builder.RegisterType<ConnectionService>().As<IConnectionService>().InstancePerDependency();

            /*
             * LOGGER
             * */
            builder.Register(c => new LoggerFactory().CreateLogger(typeof(OrderRepository))).As<ILogger>();

            // Build the container
            Container = builder.Build();
            return Container;

        }
    }
}