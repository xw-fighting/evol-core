﻿using Evol.Domain.Events;
using Evol.Common.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Evol.Domain.Messaging
{
    public class EventBus : IEventBus
    {
        public IEventHandlerFactory _eventHandlerFactory { get; private set; }

        public ILogger _logger { get; private set; }

        public EventBus(IEventHandlerFactory eventHandlerFactory, ILoggerFactory loggerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory;
            _logger = loggerFactory.CreateLogger<EventBus>();
        }
        
        public async Task PublishAsync<T>(T @event) where T : Event
        {
            var handlers = _eventHandlerFactory.GetHandler<T>();
            foreach (var handler in handlers)
            {
                var method = handler.GetType().GetMethod(nameof(handler.HandleAsync));
                var handleAsync = Attribute.GetCustomAttribute(method, typeof(ForkHandleAttribute), false) as ForkHandleAttribute;
                if (handleAsync != null)
                    ForkHandle(() => handler.HandleAsync(@event));
                else
                    await handler.HandleAsync(@event);
            }
        }

        private void ForkHandle(Action action)
        {
            var cancelSource = new CancellationTokenSource();
            Task.Run(action, cancelSource.Token)
                .ContinueWith(t => {
                    if (t.Exception == null)
                        return;
                    _logger.LogError(t.Exception, $"在处理{nameof(ForkHandleAttribute)}标记的异步方法时发生异常");
                });
        }
    }
}
