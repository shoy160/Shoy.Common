using System;

namespace com.shoy.dubbo.model
{
    public class User
    {
        private long id;
        private String name;
        private String email;
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
