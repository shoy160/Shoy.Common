using System;
using Newtonsoft.Json;

namespace com.dayeasy.service.paper.model
{
    public class PaperDto
    {
        [JsonProperty]
        private string paperId;
        [JsonProperty]
        private string paperTitle;
        [JsonProperty]
        private string paperNo;
        [JsonProperty]
        private byte paperType;
        [JsonProperty]
        private DateTime creationTime;

        public string getPaperId()
        {
            return paperId;
        }

        public void setPaperId(string paperId)
        {
            this.paperId = paperId;
        }

        public string getPaperTitle()
        {
            return paperTitle;
        }

        public void setPaperTitle(string paperTitle)
        {
            this.paperTitle = paperTitle;
        }

        public string getPaperNo()
        {
            return paperNo;
        }

        public void setPaperNo(string paperNo)
        {
            this.paperNo = paperNo;
        }

        public byte getPaperType()
        {
            return paperType;
        }

        public void setPaperType(byte paperType)
        {
            this.paperType = paperType;
        }

        public DateTime getCreationTime()
        {
            return creationTime;
        }

        public void setCreationTime(DateTime creationTime)
        {
            this.creationTime = creationTime;
        }
    }
}