﻿using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.TestFramework;
using MassTransit.Testing;
using MassTransit.Transports;
using NUnit.Framework;
using RabbitMQ.Client;

namespace BigChange.MassTransit.AwsKeyManagementService.Tests.RabbitMq
{
    public class RabbitMqTestFixture :
        BusTestFixture
    {
        public RabbitMqTestFixture(Uri logicalHostAddress = null, string inputQueueName = null)
            : this(new RabbitMqTestHarness(inputQueueName), logicalHostAddress)
        {
        }

        public RabbitMqTestFixture(RabbitMqTestHarness harness, Uri logicalHostAddress = null)
            : base(harness)
        {
            RabbitMqTestHarness = harness;

            if (logicalHostAddress != null)
            {
                RabbitMqTestHarness.NodeHostName = RabbitMqTestHarness.HostAddress.Host;
                RabbitMqTestHarness.HostAddress = logicalHostAddress;
            }

            RabbitMqTestHarness.OnConfigureRabbitMqHost += ConfigureRabbitMqHost;
            RabbitMqTestHarness.OnConfigureRabbitMqBus += ConfigureRabbitMqBus;
            RabbitMqTestHarness.OnConfigureRabbitMqBusHost += ConfigureRabbitMqBusHost;
            RabbitMqTestHarness.OnConfigureRabbitMqReceiveEndoint += ConfigureRabbitMqReceiveEndpoint;
            RabbitMqTestHarness.OnCleanupVirtualHost += OnCleanupVirtualHost;
        }

        protected RabbitMqTestHarness RabbitMqTestHarness { get; }


        /// <summary>
        ///     The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => RabbitMqTestHarness.InputQueueSendEndpoint;

        protected Uri InputQueueAddress => RabbitMqTestHarness.InputQueueAddress;

        protected Uri HostAddress => RabbitMqTestHarness.HostAddress;

        /// <summary>
        ///     The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => RabbitMqTestHarness.BusSendEndpoint;

        protected ISentMessageList Sent => RabbitMqTestHarness.Sent;

        protected Uri BusAddress => RabbitMqTestHarness.BusAddress;

        protected IRabbitMqHost Host => RabbitMqTestHarness.Host;

        protected IMessageNameFormatter NameFormatter => RabbitMqTestHarness.NameFormatter;

        [OneTimeSetUp]
        public Task SetupInMemoryTestFixture()
        {
            return RabbitMqTestHarness.Start();
        }

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return RabbitMqTestHarness.Stop();
        }

        protected virtual void ConfigureRabbitMqHost(IRabbitMqHostConfigurator configurator)
        {
        }

        protected virtual void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator,
            IRabbitMqHost host)
        {
        }

        protected virtual void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
        }

        protected virtual void OnCleanupVirtualHost(IModel model)
        {
        }
    }
}