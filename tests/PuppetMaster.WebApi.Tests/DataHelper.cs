using PuppetMaster.WebApi.Models.Database;

namespace PuppetMaster.WebApi.Tests
{
    public static class DataHelper
    {
        public static Room GetRoom()
        {
            return new Room();
        }

        public static Match GetMatch()
        {
            return new Match();
        }
    }
}
