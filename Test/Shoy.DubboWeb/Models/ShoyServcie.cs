
using com.dayeasy.service.paper.api;
using com.shoy.dubbo.model;

namespace com.shoy.dubbo.api
{
    public interface ShoyService:IDubboService
    {
        string sayHello(string name);
        int add(int a, int b);
        User getUser();
    }
}
