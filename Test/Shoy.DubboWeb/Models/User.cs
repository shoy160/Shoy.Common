using System;
using Newtonsoft.Json;

namespace com.shoy.dubbo.model
{
    public class User
    {
        [JsonProperty]
        private long id;
        [JsonProperty]
        private String name;
        [JsonProperty]
        private String email;
        [JsonProperty]
        private DateTime creation;

        public long getId()
        {
            return id;
        }

        public void setId(long id)
        {
            this.id = id;
        }

        public String getName()
        {
            return name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public String getEmail()
        {
            return email;
        }

        public void setEmail(String email)
        {
            this.email = email;
        }

        public DateTime getCreation()
        {
            return creation;
        }

        public void setCreation(DateTime creation)
        {
            this.creation = creation;
        }
    }
}