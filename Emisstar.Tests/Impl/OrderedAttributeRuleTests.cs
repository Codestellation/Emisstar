using Codestellation.Emisstar.Impl;
using NUnit.Framework;

namespace Codestellation.Emisstar.Tests.Impl
{
    [TestFixture]
    public class OrderedAttributeRuleTests
    {
        [OrderedExecution]
        private class MarkedMessage {}

        private class NonMarkedMessage {}

        [Test]
        public void Retruns_false_if_not_marked_ordered()
        {
            var rule = new OrderedRule();
            var message = new NonMarkedMessage(); 

            Assert.That(rule.CanDispatch(message, null), Is.False);
        }

        [Test]
        public void Retruns_true_if_marked_ordered()
        {
            var rule = new OrderedRule();
            var message = new MarkedMessage();

            Assert.That(rule.CanDispatch(message, null), Is.True);
        }
    }
}