using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 public   class AccountMergeInfo2Client : CodeMessageBase
{
    /// <summary>
    /// 要绑定账户是否已存在一个单独账户
    /// </summary>
    public bool alreadyExistAccount;
    /// <summary>
    /// 当要绑定账户已存在，User数据
    /// </summary>
    public User mergeAccount;
  
   
    public override  void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}