using Microsoft.Extensions.DependencyInjection;
using System;
using Proto;
using Xunit;

namespace Proto.ActorExtensions.Tests
{
    public class ActorFactoryTest
    {
        [Fact]
        public void SpawnActor()
        {
            var services = new ServiceCollection();
            services.AddProtoActor();

            var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<IActorFactory>();

            var pid = factory.GetActor<SampleActor>();

            pid.Tell("hello");
            pid.Stop();

            Assert.True(SampleActor.Created);
        }

        [Fact]
        public void should_register_by_type()
        {
            var services = new ServiceCollection();
            var created = false;

            Func<IActor> producer = () =>
            {
                created = true;
                return new SampleActor();
            };

            services.AddProtoActor(register => register.RegisterProps(typeof(SampleActor), p => p.WithProducer(producer)));

            var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<IActorFactory>();

            var pid = factory.GetActor<SampleActor>();

            pid.Stop();

            Assert.True(created);
        }

        [Fact]
        public void should_throw_if_not_actor_type()
        {
            var services = new ServiceCollection();
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                services.AddProtoActor(register => register.RegisterProps(GetType(), p => p));
            });

            Assert.Equal($"Type {GetType().FullName} must implement {typeof(IActor).FullName}", ex.Message);
        }
    }
}