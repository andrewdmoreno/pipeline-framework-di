﻿using Autofac;
using PipelineFramework.Abstractions;
using System.Collections.Generic;

namespace PipelineFramework.Autofac.Examples
{
    public class PipelineModule : PipelineModuleBase
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Component1>()
                .Named<IAsyncPipelineComponent<ExamplePipelinePayload>>(typeof(Component1).Name);
            builder.RegisterType<Component2>()
                .Named<IAsyncPipelineComponent<ExamplePipelinePayload>>(typeof(Component2).Name);
            builder.RegisterType<Component3>()
                .Named<IAsyncPipelineComponent<ExamplePipelinePayload>>(typeof(Component3).Name);

            builder.RegisterInstance(new Dictionary<string, IDictionary<string, string>>())
                .As<IDictionary<string, IDictionary<string, string>>>();

            builder.Register(context =>
                PipelineBuilder<ExamplePipelinePayload>
                    .Async()
                    .WithComponent<Component1>()
                    .WithComponent<Component2>()
                    .WithComponent<Component3>()
                    .WithComponentResolver(context.Resolve<IPipelineComponentResolver>())
                    .WithSettings(context.Resolve<IDictionary<string, IDictionary<string, string>>>())
                    .Build())
                .As<IAsyncPipeline<ExamplePipelinePayload>>();
        }
    }
}
