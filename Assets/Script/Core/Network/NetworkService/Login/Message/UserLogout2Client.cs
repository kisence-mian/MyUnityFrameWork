using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UserLogout2Client : MessageClassInterface
{
    public void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}

