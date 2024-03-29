﻿using DotNetCore.CAP.Dashboard;
using Hao.Core.Extensions;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class H_Cap
    {
        public static IServiceCollection AddCapService(this IServiceCollection services, H_CapConfig config)
        {
            services.AddCap(x =>
            {
                x.UseDashboard(a =>
                {
                    a.PathMatch = "/haohaoplay_back_capdashboard"; //地址http://localhost:8000/haohaoplay_back_capdashboard
                }); 

                x.UsePostgreSql(cfg => { cfg.ConnectionString = config.PostgreSqlConnection; });

                x.UseRabbitMQ(cfg =>
                {
                    cfg.HostName = config.HostName;
                    cfg.VirtualHost = config.VirtualHost; // 相当于数据库 可以在rabbitmq管理后台里面进行添加
                    cfg.Port = config.Port;
                    cfg.UserName = config.UserName;
                    cfg.Password = config.Password;
                });

                //订阅者组的概念类似于 Kafka 中的消费者组，它和消息队列中的广播模式相同，用来处理不同微服务实例之间同时消费相同的消息。

                //当CAP启动的时候，她将创建一个默认的消费者组，如果多个相同消费者组的消费者消费同一个Topic消息的时候，只会有一个消费者被执行。
                 
                //相反，如果消费者都位于不同的消费者组，则所有的消费者都会被执行。

                //相同的实例中，你可以通过下面的方式来指定他们位于不同的消费者组。
                x.DefaultGroupName = config.DefaultGroupName;

                x.FailedRetryCount = 10; //失败重试机会，默认50次
                x.FailedRetryInterval = 30; //每60秒重试一次   50次*60秒  50分钟后放弃失败重试
                //x.SucceedMessageExpiredAfter = 24 * 3600;
                // If you are using Kafka, you need to add the configuration：
                //x.UseKafka("localhost");
            });

            return services;
        }
    }

    //internal class CapDashboardFilter : IDashboardAuthorizationFilter
    //{
    //    //默认地址http://localhost:8000/cap?password=666666
    //    public async Task<bool> AuthorizeAsync(DashboardContext context)
    //    {
    //        return await Task.Factory.StartNew(() =>
    //        {
    //            if (context.Request.GetQuery("password") == "666666")
    //            {
    //                return true;
    //            }
    //            return false;
    //        });
    //    }
    //}
}

