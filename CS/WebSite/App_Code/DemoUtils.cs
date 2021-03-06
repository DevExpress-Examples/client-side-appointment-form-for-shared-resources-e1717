using System;
using System.Text;
using System.Xml.XPath;
using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Xml;
using System.Collections.Generic;
using DevExpress.Schedule;
using DevExpress.Schedule.Serializing;


public class DemoUtils {
    static DateTime baseDate = new DateTime(2008, 7, 13);
    public static Random RandomInstance = new Random();
    public static string[] Users = new string[] { "Peter Dolan", "Ryan Fischer", "Andrew Miller", "Tom Hamlett",
												"Jerry Campbell", "Carl Lucas", "Mark Hamilton", "Steve Lee" };

    public static DateTime BaseDate { get { return baseDate; } }
    public static void FillResources(ASPxSchedulerStorage storage, int count) {
        ResourceCollection resources = storage.Resources.Items;
        storage.BeginUpdate();
        try {
            int cnt = Math.Min(count, Users.Length);
            for (int i = 1; i <= cnt; i++) {
                Resource resource = storage.CreateResource(i);
                resource.Caption = Users[i - 1];
                resources.Add(resource);
            }
        }
        finally {
            storage.EndUpdate();
        }
    }
    public static HolidayBaseCollection LoadHolidaysFromXml(string uri, string location) {
        XPathDocument document = new XPathDocument(uri);
        XPathNavigator navigator = document.CreateNavigator();

        string xPath = String.Format("/Holidays/Holiday[@Location='{0}']", location);
        XPathNodeIterator nodes = navigator.Select(xPath);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<Holidays>");
        while (nodes.MoveNext()) {
            sb.AppendLine(nodes.Current.OuterXml);
        }
        sb.AppendLine("</Holidays>");

        return HolidayCollectionXmlPersistenceHelper.ObjectFromXml(sb.ToString());
    }


    #region Custom Events
    public static CustomEventList CreateCustomEvents() {
        return CreateCustomEvents(false);
    }
    public static CustomEventList CreateCustomEvents(bool resourceSharing) {
        CustomEventList events = new CustomEventList();
        PopulateRentals(events, resourceSharing);
        PopulateMaintenance(events, resourceSharing);
        return events;
    } 
    static void PopulateRentals(CustomEventList events, bool resourceSharing) {
        string[] customers = new string[] { "Mr.Brown", "Mr.White", "Mrs.Black", "Mr.Green" };
        string[] locations = new string[] { "city", "out of town" };
        int count = 20;
        for (int i = 0; i < count; i++)
            events.Add(CreateRental(customers, locations, resourceSharing));
    }
    static CustomEvent CreateRental(string[] customers, string[] locations, bool resourceSharing) {
        Random rnd = DemoUtils.RandomInstance;

        CustomEvent result = new CustomEvent();
        result.Subject = customers[rnd.Next(0, customers.Length)];
        result.Location = locations[rnd.Next(0, locations.Length)];
        result.Description = "Rent this car";

        int rangeInDays = 7;
        int rangeInHours = 18;
        TimeSpan offset = TimeSpan.FromDays(rnd.Next(0, rangeInDays)) + TimeSpan.FromHours(rnd.Next(0, rangeInHours));
        result.StartTime = baseDate + offset;
        result.EndTime = result.StartTime + TimeSpan.FromHours(rnd.Next(1, rangeInHours));
        result.OwnerId = CalculateResourceIdOrIds(resourceSharing);
        result.Status = 2;
        result.Label = rnd.Next(0, 7);
        result.Id = Guid.NewGuid();
        AddEventAdditionalInfo(result);
        return result;
    }
    static void PopulateMaintenance(CustomEventList events, bool resourceSharing) {
        CustomEvent wash = new CustomEvent();
        wash.Subject = "Wash";
        wash.Description = "Wash this car in the garage";
        wash.Location = "Garage";
        wash.StartTime = baseDate + TimeSpan.FromHours(7);
        wash.EndTime = baseDate + TimeSpan.FromHours(8);
        wash.OwnerId = CalculateResourceIdOrIds(resourceSharing);
        wash.Status = 1;
        wash.Label = 2;
        wash.EventType = 1;
        wash.RecurrenceInfo = @"<RecurrenceInfo AllDay=""False"" DayNumber=""1"" DayOfMonth=""0"" WeekDays=""42"" Id=""51c81018-53fa-4d10-925f-2ed7f8408c75"" Month=""12"" OccurenceCount=""19"" Periodicity=""1"" Range=""2"" Start=""7/11/2008 7:00:00"" End=""8/24/2008 1:00:00"" Type=""1"" />";
        wash.Id = Guid.NewGuid();
        AddEventAdditionalInfo(wash);
        events.Add(wash);
    }
    static void AddEventAdditionalInfo(CustomEvent ev) {
        string[] info = new string[] {
				"Email: info{0}@wash_garage.com",
				"cellular: +530145961{0}",
				"Address: WA Seattle {0} - 24th Ave. S.Suite 4B phone: (206) 555-4{0}",
				"Contact: Address: OR Elgin City Center Plaza {0} Main St.",
				"Phone: (262) 946-9{0}; w ({0}) 723-2678 x22, cell (253) 713-0{0}, fax (361) 733-2{0}" 
		};
        Random rnd = DemoUtils.RandomInstance;

        ev.Price = rnd.Next(5, 100);

        string infoTemplate = info[rnd.Next(0, info.Length)];
        Char ch = Char.Parse(rnd.Next(1, 9).ToString());
        ev.ContactInfo = string.Format(infoTemplate, new String(ch, 3));
    }
    static object CalculateResourceIdOrIds(bool resourceSharing) {
        if (resourceSharing) {
            List<int> resourceIdList = GetRandomResourceIdSequence();
            return GenerateResourceIdsString(resourceIdList.ToArray());
        }
        else
            return GetRandomResourceId();
    }
    private static List<int> GetRandomResourceIdSequence() {
        int expectedCount = GetRandomResourceId();
        List<int> result = new List<int>();
        for (int i = 0; i < expectedCount; i++) {
            int tryCounter = 0;
            while (true) {
                int id = GetRandomResourceId();
                if (result.IndexOf(id) == -1) {
                    result.Add(id);
                    break;
                }
                if (tryCounter > 10)
                    break;
                tryCounter++;
            }
        }
        return result;
    }
    static int GetRandomResourceId() {
        return DemoUtils.RandomInstance.Next(1, 6);
    }
    static string GenerateResourceIdsString(int[] resourceIds) {
        ResourceIdCollection resourceIdCollection = new ResourceIdCollection();
        int count = resourceIds.Length;
        for (int i = 0; i < count; i++)
            resourceIdCollection.Add(resourceIds[i]);
        AppointmentResourceIdCollectionXmlPersistenceHelper helper = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIdCollection);
        return helper.ToXml();
    }
    #endregion

    #region Custom Resources
    public static CustomResourceList CreateCustomResurces() {
        CustomResourceList resources = new CustomResourceList();
        resources.Add(CreateCustomResource(1, "SL500 Roadster"));
        resources.Add(CreateCustomResource(2, "CLK55 AMG Cabriolet"));
        resources.Add(CreateCustomResource(3, "C230 Kompressor Sport Coupe"));
        resources.Add(CreateCustomResource(4, "530i"));
        resources.Add(CreateCustomResource(5, "Corniche"));
        return resources;
    }  
    static CustomResource CreateCustomResource(int id, string caption) {
        CustomResource result = new CustomResource();
        result.Id = id;
        result.Caption = caption;
        return result;
    }
    #endregion
}
