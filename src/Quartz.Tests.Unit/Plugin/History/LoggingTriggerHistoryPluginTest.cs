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

using System.Collections.Generic;

using NUnit.Framework;

using Quartz.Impl;
using Quartz.Job;
using Quartz.Plugin.History;
using Quartz.Spi;
using Quartz.Logging;

using Rhino.Mocks;

namespace Quartz.Tests.Unit.Plugin.History
{
    /// <author>Marko Lahma (.NET)</author>
    [TestFixture]
    public class LoggingTriggerHistoryPluginTest
    {
        private RecordingLoggingTriggerHistoryPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new RecordingLoggingTriggerHistoryPlugin();
        }

        [Test]
        public void TestTriggerFiredMessage()
        {
            // arrange
            ITrigger t = TriggerBuilder.Create()
                                        .WithSchedule(SimpleScheduleBuilder.Create())
                                        .Build();
            
            IJobExecutionContext ctx = new JobExecutionContextImpl(
                null, 
                TestUtil.CreateMinimalFiredBundleWithTypedJobDetail(typeof(NoOpJob), (IOperableTrigger) t), 
                null);

            // act
            plugin.TriggerFired(t, ctx);

            // assert
            Assert.That(plugin.InfoMessages.Count, Is.EqualTo(1));
        }


        [Test]
        public void TestTriggerMisfiredMessage()
        {
            // arrange
            IOperableTrigger t = (IOperableTrigger) TriggerBuilder.Create()
                                                        .WithSchedule(SimpleScheduleBuilder.Create())
                                                        .Build();

            t.JobKey = new JobKey("name", "group");
            
            // act
            plugin.TriggerMisfired(t);

            // assert
            Assert.That(plugin.InfoMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestTriggerCompleteMessage()
        {
            // arrange
            ITrigger t = TriggerBuilder.Create()
                                        .WithSchedule(SimpleScheduleBuilder.Create())
                                        .Build();
            
            IJobExecutionContext ctx = new JobExecutionContextImpl(
                null,
                TestUtil.CreateMinimalFiredBundleWithTypedJobDetail(typeof(NoOpJob), (IOperableTrigger) t),
                null);

            // act
            plugin.TriggerComplete(t, ctx, SchedulerInstruction.ReExecuteJob);

            // assert
            Assert.That(plugin.InfoMessages.Count, Is.EqualTo(1));
        }

        private class RecordingLoggingTriggerHistoryPlugin : LoggingTriggerHistoryPlugin
        {
            public RecordingLoggingTriggerHistoryPlugin()
            {
                InfoMessages = new List<string>();
            }
            public List<string> InfoMessages { get; private set; }

            protected override bool IsInfoEnabled  { get { return true; } }

            protected override void WriteInfo(string message)
            {
                InfoMessages.Add(message);
            }
        }

    }
}
