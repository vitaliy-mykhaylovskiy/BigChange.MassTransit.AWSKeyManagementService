using GreenPipes;
using MassTransit;

namespace BigChange.MassTransit.AwsKeyManagementService.Tests
{
	public interface IConsumePipeBuilder :
		IPipeBuilder<ConsumeContext>
	{
		/// <summary>
		/// Add a filter to the pipe after any existing filters
		/// </summary>
		/// <param name="filter">The filter to add</param>
		void AddFilter<T>(IFilter<ConsumeContext<T>> filter)
			where T : class;
	}
}