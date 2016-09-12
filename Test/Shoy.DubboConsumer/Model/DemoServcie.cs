
//using com.shoy.dubbo.model;

using com.shoy.dubbo.model;

namespace com.shoy.dubbo.api
{
    public interface ShoyService
    {
        string sayHello(string name);
        int add(int a, int b);
        User getUser();
    }
}
