using System;

namespace BigChange.MassTransit.AwsKeyManagementService.Tests
{
	public interface ComplaintAdded
	{
		User AddedBy { get; }

		DateTime AddedAt { get; }

		string Subject { get; }

		string Body { get; }

		BusinessArea Area { get; }
	}
}