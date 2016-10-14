
using com.dayeasy.service.paper.model;

namespace com.dayeasy.service.paper.api
{
    public interface IPaperService : IDubboService
    {
        string createPaper();
        PaperDto getPaper(string id);
    }
}