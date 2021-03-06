﻿using LightInject.Interception;
using PipelineFramework.Abstractions;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PipelineFramework.LightInject.Logging
{
    public class AsyncPipelineComponentInterceptor : AsyncInterceptor
    {
        protected ILogger Logger { get; }

        public AsyncPipelineComponentInterceptor(ILogger logger) 
            : base(null)
        {
            Logger = logger;
        }

        protected override async Task<T> InvokeAsync<T>(IInvocationInfo invocationInfo)
        {
            var logger = EnrichLogger(invocationInfo);
            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                var result = await base.InvokeAsync<T>(invocationInfo);
                stopwatch.Stop();
                logger.Information(LogMessageTemplates.SuccessMessage, stopwatch.ElapsedMilliseconds);
                return result;
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                logger.Error(e, LogMessageTemplates.ExceptionMessage, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        protected virtual ILogger EnrichLogger(IInvocationInfo invocationInfo)
        {
            return invocationInfo.Proxy.Target is IPipelineComponent component
                ? Logger.ForContext(ComponentNameLoggingLabel, component.Name)
                : Logger;
        }

        public virtual string ComponentNameLoggingLabel => "ComponentName";
    }
}
