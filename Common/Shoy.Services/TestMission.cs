using System.Text;

namespace Shoy.Services
{
    public class TestMission : MissionBase
    {
        public override void Start()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < 1000; i++)
            {
                if (IsAbort)
                    break;
                sb.AppendLine(string.Format("Test 输出 : {0}", i));
            }
        }
    }
}
