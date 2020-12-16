using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

using Autofac.Extras.FakeItEasy;

using Castle.DynamicProxy;

using FakeItEasy;

using FluentAssertions;

using Functionless.Durability;

namespace Functionless.Tests.Durability
{
    [TestClass]
    public class DurableInterceptorTests
    {
        /**
         * Context    Attribute              Depth  Action
         * --------------------------------------------------------------
         * Yes        Orchestration          =1     Proceed
         * Yes        Orchestration          >1     Durable-Orchestration
         * Yes        Activity               =1     Durable-Activity
         * Yes        Activity               >1     Durable-Activity
         * Yes        OrchestrationActivity  =1     Durable-Activity
         ! Yes        OrchestrationActivity  >1     Durable-Orchestration
         * No         ?                      ?      Proceed
         */
        [DataTestMethod]
        [DataRow(true, "Orchestration", 0, true, null)]
        [DataRow(true, "Orchestration", 1, false, "CallSubOrchestratorAsync")]
        [DataRow(true, "Activity", 0, false, "CallActivityAsync")]
        [DataRow(true, "Activity", 1, false, "CallActivityAsync")]
        [DataRow(true, "OrchestrationActivity", 0, false, "CallActivityAsync")]
        [DataRow(true, "OrchestrationActivity", 1, false, "CallSubOrchestratorAsync")]
        [DataRow(false, "Orchestration", 0, true, null)]
        [DataRow(false, "Orchestration", 1, true, null)]
        [DataRow(false, "Activity", 0, true, null)]
        [DataRow(false, "Activity", 1, true, null)]
        [DataRow(false, "OrchestrationActivity", 0, true, null)]
        [DataRow(false, "OrchestrationActivity", 1, true, null)]
        public void InterceptTest(bool hasContext, string method, int initialDepth, bool proceedCalled, string durableInvocation)
        {
            using (var fake = new AutoFake())
            {
                DurableInterceptor.Context.Value = hasContext ? new DurableContext { OrchestrationContext = fake.Resolve<IDurableOrchestrationContext>() } : null;

                var invocation = fake.Resolve<IInvocation>();

                A.CallTo(() => invocation.Method).Returns(this.GetType().GetMethod(method));

                var durableInterceptor = fake.Resolve<DurableInterceptor>();

                (durableInterceptor.GetType()
                    .GetMethod("get_Depth", BindingFlags.NonPublic | BindingFlags.Static)
                    .Invoke(durableInterceptor, null) as AsyncLocal<int>
                ).Value = initialDepth;

                durableInterceptor.Intercept(invocation);

                A.CallTo(() => invocation.Proceed()).MustHaveHappened(proceedCalled ? 1 : 0, Times.Exactly);

                if (!string.IsNullOrWhiteSpace(durableInvocation))
                {
                    Fake.GetCalls(DurableInterceptor.Context.Value.OrchestrationContext)
                        .Any(p => p.Method.Name == durableInvocation)
                        .Should().BeTrue();
                }
            }
        }

        [SubOrchestration]
        public virtual Task Orchestration()
        {
            return Task.CompletedTask;
        }

        [Activity]
        public virtual Task Activity()
        {
            return Task.CompletedTask;
        }

        [SubOrchestration(true), Activity]
        public virtual Task OrchestrationActivity()
        {
            return Task.CompletedTask;
        }
    }
}
