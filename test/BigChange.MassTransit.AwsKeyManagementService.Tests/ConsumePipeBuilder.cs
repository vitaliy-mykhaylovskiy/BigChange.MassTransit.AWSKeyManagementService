using System;
using GreenPipes;
using GreenPipes.Builders;
using GreenPipes.Filters;
using MassTransit;
using MassTransit.Context.Converters;
using MassTransit.Pipeline;
using MassTransit.Pipeline.Pipes;

namespace BigChange.MassTransit.AwsKeyManagementService.Tests
{
    public class ConsumePipeBuilder :
        IConsumePipeBuilder
    {
        private readonly DynamicFilter<ConsumeContext, Guid> _filter;
        private readonly PipeBuilder<ConsumeContext> _pipeBuilder;

        public ConsumePipeBuilder()
        {
            _filter = new DynamicFilter<ConsumeContext, Guid>(new ConsumeContextConverterFactory(), GetRequestId);
            _pipeBuilder = new PipeBuilder<ConsumeContext>();
        }

        void IPipeBuilder<ConsumeContext>.AddFilter(IFilter<ConsumeContext> filter)
        {
            _pipeBuilder.AddFilter(filter);
        }

        void IConsumePipeBuilder.AddFilter<T>(IFilter<ConsumeContext<T>> filter)
        {
            _filter.AddFilter(filter);
        }

        public IConsumePipe Build()
        {
            _pipeBuilder.AddFilter(_filter);

            return new ConsumePipe(_filter, _pipeBuilder.Build());
        }

        private static Guid GetRequestId(ConsumeContext context)
        {
            return context.RequestId ?? Guid.Empty;
        }
    }
}