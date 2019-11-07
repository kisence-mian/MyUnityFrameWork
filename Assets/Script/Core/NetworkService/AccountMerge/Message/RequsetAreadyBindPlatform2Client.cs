using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RequsetAreadyBindPlatform2Client : MessageClassInterface
{
    public List<LoginPlatform> areadyBindPlatforms = new List<LoginPlatform>();
    public void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}

