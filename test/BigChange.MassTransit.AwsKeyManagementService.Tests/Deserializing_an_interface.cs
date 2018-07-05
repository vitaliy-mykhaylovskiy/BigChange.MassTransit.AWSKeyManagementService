using System.Linq;
using System.Threading.Tasks;
using MassTransit.TestFramework;
using MassTransit.Testing;
using NUnit.Framework;
using Shouldly;

namespace BigChange.MassTransit.AwsKeyManagementService.Tests
{
    [TestFixture]
    public class Deserializing_an_interface :
        SerializationTest
    {
        [Test]
        public void Should_create_a_proxy_for_the_interface()
        {
            var user = new UserImpl("Chris", "noone@nowhere.com");
            ComplaintAdded complaint = new ComplaintAddedImpl(user, "No toilet paper", BusinessArea.Appearance)
            {
                Body =
                    "There was no toilet paper in the stall, forcing me to use my treasured issue of .NET Developer magazine."
            };

            var result = SerializeAndReturn(complaint);

            complaint.Equals(result).ShouldBe(true);
        }

        [Test]
        public async Task Should_dispatch_an_interface_via_the_pipeline()
        {
            var pipe = new ConsumePipeBuilder().Build();

            var consumer = new MultiTestConsumer(TestTimeout);
            consumer.Consume<ComplaintAdded>();

            consumer.Connect(pipe);

            var user = new UserImpl("Chris", "noone@nowhere.com");
            ComplaintAdded complaint = new ComplaintAddedImpl(user, "No toilet paper", BusinessArea.Appearance)
            {
                Body =
                    "There was no toilet paper in the stall, forcing me to use my treasured issue of .NET Developer magazine."
            };


            await pipe.Send(new TestConsumeContext<ComplaintAdded>(complaint));

            consumer.Received.Select<ComplaintAdded>().Any().ShouldBe(true);
        }
    }
}