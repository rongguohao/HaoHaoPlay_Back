﻿using Hao.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyServiceCollectionExtensions
    {
        /// <summary>
        /// 自动IOC扫描注入
        /// </summary>
        public static IServiceCollection AutoDependency(this IServiceCollection services, IEnumerable<Type> types)
        {
            services.Scan(scan => scan.FromAssembliesOf(types)
                                      .AddClasses(x => typeof(ITransientDependency).IsAssignableFrom(x.GetType()))  //直接或间接实现了ICapSubscribe
                                      .AsImplementedInterfaces()
                                      .WithTransientLifetime())

                    .Scan(scan => scan.FromAssembliesOf(types)
                                      .AddClasses(x => typeof(ISingletonDependency).IsAssignableFrom(x.GetType()))  //直接或间接实现了ICapSubscribe
                                      .AsImplementedInterfaces()
                                      .WithSingletonLifetime());

            //services.Scan(scan => scan.FromAssembliesOf(types)
            //              .AddClasses()
            //              .AsMatchingInterface((x, p) => typeof(ITransientDependency).IsAssignableFrom(p.GetType())) //直接或间接实现了ITransientDependency
            //              .WithTransientLifetime())

            //        .Scan(scan => scan.FromAssembliesOf(types)
            //              .AddClasses()
            //              .AsMatchingInterface((x, p) => typeof(ISingletonDependency).IsAssignableFrom(p.GetType()))
            //              .WithSingletonLifetime());
            return services;
        }

        /// <summary>
        /// 自动IOC扫描注入
        /// </summary>
        public static IServiceCollection AutoDependency(this IServiceCollection services, params Type[] types)
        {
            return services.AutoDependency(types.ToList());
        }
    }
}