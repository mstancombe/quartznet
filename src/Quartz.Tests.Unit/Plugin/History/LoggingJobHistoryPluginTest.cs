#region License
/* 
 * All content copyright Terracotta, Inc., unless otherwise indicated. All rights reserved. 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not 
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at 
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0 
 *   
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations 
 * under the License.
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

using Quartz.Logging;

using NUnit.Framework;

using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Job;
using Quartz.Plugin.History;
using Quartz.Spi;

using Rhino.Mocks;

namespace Quartz.Tests.Unit.Plugin.History
{
    /// <author>Marko Lahma (.NET)</author>
    [TestFixture]
    public class LoggingJobHistoryPluginTest
    {
        private RecordingLoggingJobHistoryPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new RecordingLoggingJobHistoryPlugin();
        }

        [Test]
        public void TestJobFailedMessage()
        {
            // act
            JobExecutionException ex = new JobExecutionException("test error");
            plugin.JobWasExecuted(CreateJobExecutionContext(), ex);

            // assert
            Assert.That(plugin.WarnMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestJobSuccessMessage()
        {
            // act
            plugin.JobWasExecuted(CreateJobExecutionContext(), null);

            // assert
            Assert.That(plugin.InfoMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestJobToBeFiredMessage()
        {
            // act
            plugin.JobToBeExecuted(CreateJobExecutionContext());

            // assert
            Assert.That(plugin.InfoMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestJobWasVetoedMessage()
        {
            // act
            plugin.JobExecutionVetoed(CreateJobExecutionContext());

            // assert
            Assert.That(plugin.InfoMessages.Count, Is.EqualTo(1));
        }

        protected virtual IJobExecutionContext CreateJobExecutionContext()
        {
            IOperableTrigger t = new SimpleTriggerImpl("name", "group");
            TriggerFiredBundle firedBundle = TestUtil.CreateMinimalFiredBundleWithTypedJobDetail(typeof(NoOpJob), t);
            IJobExecutionContext ctx = new JobExecutionContextImpl(null,  firedBundle, null);

            return ctx;
        }

        private class RecordingLoggingJobHistoryPlugin : LoggingJobHistoryPlugin
        {
            public List<string> InfoMessages { get; } = new List<string>();
            public List<string> WarnMessages { get; } = new List<string>();

            protected override bool IsInfoEnabled => true;
            protected override bool IsWarnEnabled => true;

            protected override void WriteInfo(string message)
            {
                InfoMessages.Add(message);
            }

            protected override void WriteWarning(string message, Exception ex)
            {
                WarnMessages.Add(message);
            }
        }
    }
}
