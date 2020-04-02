using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// 刷新的商店信息
/// </summary>
public class UpdateGeneralShopInfo2Client : MessageClassInterface
{
    /// <summary>
    /// 刷新的商店信息
    /// </summary>
    public GameShopInfoData shopInfo = null;

    public void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }

    public GameShopInfoData getShopInfo()
    {
        return shopInfo;
    }

    public void setShopInfo(GameShopInfoData shopInfo)
    {
        this.shopInfo = shopInfo;
    }




}
