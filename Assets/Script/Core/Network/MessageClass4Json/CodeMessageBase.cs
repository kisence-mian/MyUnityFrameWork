using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class CodeMessageBase : MessageClassInterface
{
    public int code;
    public string e;
    public abstract void DispatchMessage();
}
