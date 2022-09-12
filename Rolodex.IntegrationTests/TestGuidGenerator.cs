using Rolodex.Web.Infrastructure;

namespace Rolodex.IntegrationTests
{
    public class TestGuidGenerator : IGuidGenerator
    {
        public Guid? TestingGuid { get; set; }

        public Guid NewGuid()
        {
            return TestingGuid ?? new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd");
        }
    }
}
