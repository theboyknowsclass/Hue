using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Tests.Models
{
    [TestClass]
    public class ScheduleTests
    {

        [TestMethod]
        public async Task GetAttributesTest()
        {
            var bridge = await BridgeTests.GetLoggedInBridge();

            var command = new Command { Address = "/api/0/groups/1/action", Method = "PUT" };
            command.Body.Add("on", true);
            command.Body.Add("Hue", 25400);
            command.Body.Add("Brightness", 254);
            command.Body.Add("Saturation", 254);

            var schedule = new Schedule(command, DateTime.Now.AddMinutes(1)) { Description = GetRandomString(), Name = "test schedule" };

            var rv = await bridge.CreateScheduleAsync(schedule) as HueObjectCollectionBase<Success>;

            Assert.IsNotNull(rv);

            string id = (from kvp in rv.Dictionary select kvp.Value.PathValue).FirstOrDefault();

            Assert.IsNotNull(id);

            var schedules = await bridge.GetSchedulesAsync() as HueObjectCollectionBase<Schedule>;

            Assert.IsNotNull(schedules);

            var createdSchedule = schedules.Dictionary[id];

            Assert.IsNotNull(createdSchedule);

            createdSchedule = await createdSchedule.GetAttributesAsync() as Schedule;
            
            Assert.IsNotNull(createdSchedule);

            Assert.AreEqual(schedule.Description, createdSchedule.Description);

            Assert.IsInstanceOfType(await bridge.DeleteScheduleAsync(id), typeof(HueObjectCollectionBase<Success>));
        }

        [TestMethod]
        public async Task SetAttributesTest()
        {
            var bridge = await BridgeTests.GetLoggedInBridge();

            var command = new Command { Address = "/api/0/groups/1/action", Method = "PUT" };
            command.Body.Add("on", true);
            command.Body.Add("Hue", 25400);
            command.Body.Add("Brightness", 254);
            command.Body.Add("Saturation", 254);

            var schedule = new Schedule(command, DateTime.Now.AddMinutes(1)) { Description = "test schedule " + GetRandomString(), Name = "test schedule" };

            var rv = await bridge.CreateScheduleAsync(schedule) as HueObjectCollectionBase<Success>;

            Assert.IsNotNull(rv);

            string id = (from kvp in rv.Dictionary select kvp.Value.PathValue).FirstOrDefault();

            Assert.IsNotNull(id);

            var schedules = await bridge.GetSchedulesAsync() as HueObjectCollectionBase<Schedule>;

            Assert.IsNotNull(schedules);

            var createdSchedule = schedules.Dictionary[id];

            Assert.IsNotNull(createdSchedule);

            createdSchedule = await createdSchedule.GetAttributesAsync() as Schedule;

            Assert.IsNotNull(createdSchedule);

            Assert.AreEqual(schedule.Description, createdSchedule.Description);

            schedule = new Schedule { Description = "new description" };

            Assert.IsInstanceOfType(await createdSchedule.SetAttributesAsync(schedule), typeof(HueObjectCollectionBase<Success>));

            var alteredSchedule = await createdSchedule.GetAttributesAsync() as Schedule;

            Assert.IsNotNull(alteredSchedule);

            Assert.AreEqual(schedule.Description, alteredSchedule.Description);

            Assert.IsInstanceOfType(await bridge.DeleteScheduleAsync(id), typeof(HueObjectCollectionBase<Success>));
        }

        private string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }
    }


}
