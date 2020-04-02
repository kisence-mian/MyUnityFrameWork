using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/***
 * 购买次数限制
 * @author xgs2
 *
 */
public class GoodsBuyNumOfTimeLimitInfo
{

    /***
	 * 购买次数限制（-1时表示不限制）
	 */
    public int buyTimesLimit = -1;
    /***
	 * 当前限制范围内已购买次数,可能重置(比如每周购买3次，下周将重置数目)
	 */
    public int alreadyBuyTimes = 0;
    /***
	 * 总共购买次数（不会重置）
	 */
    public int totalBuyTimes = 0;


    public int getBuyTimesLimit()
    {
        return buyTimesLimit;
    }
    public void setBuyTimesLimit(int buyTimesLimit)
    {
        this.buyTimesLimit = buyTimesLimit;
    }
    public int getAlreadyBuyTimes()
    {
        return alreadyBuyTimes;
    }
    public void setAlreadyBuyTimes(int alreadyBuyTimes)
    {
        this.alreadyBuyTimes = alreadyBuyTimes;
    }
    public int getTotalBuyTimes()
    {
        return totalBuyTimes;
    }
    public void setTotalBuyTimes(int totalBuyTimes)
    {
        this.totalBuyTimes = totalBuyTimes;
    }
}