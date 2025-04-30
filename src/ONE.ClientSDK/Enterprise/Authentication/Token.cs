using System;

namespace ONE.ClientSDK.Enterprise.Authentication
{
    public class Token
    {
        public Token()
        {
            created = DateTime.Now;
        }
        //{"access_token":"fe8d752f650151e03ce5fb903117f3eed032906b3a48bd03ca4677b136dfc5d0","expires_in":86400,"token_type":"Bearer","scope":"FFAccessAPI openid"}
        public string access_token { get; set; }
        public long expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public DateTime created { get; set; }

        private DateTime _expires;
        public DateTime expires 
        {
            get
            {
                if (_expires == DateTime.MinValue)
                {
                    _expires = created.AddSeconds(expires_in);
                }
                return _expires;
            }
            set
            {
                _expires = value;
            }
        }
    }
}
