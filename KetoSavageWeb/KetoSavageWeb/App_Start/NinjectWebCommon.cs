﻿[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(KetoSavageWeb.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(KetoSavageWeb.App_Start.NinjectWebCommon), "Stop")]

namespace KetoSavageWeb.App_Start
{
    using KetoSavageWeb.Domain.Infrastructure;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Models;
    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;


    public class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<System.Data.Entity.DbContext>().To<KSDataContext>().InRequestScope();
            kernel.Bind(typeof(IEntityContext<>)).To(typeof(EfEntityContext<>));
            //kernel.Bind<IMailHelper>().To<SmtpMailHelper>();
            //kernel.Bind<IMessageTemplateService>().To<MessageTemplateService>();
        }
    }
}