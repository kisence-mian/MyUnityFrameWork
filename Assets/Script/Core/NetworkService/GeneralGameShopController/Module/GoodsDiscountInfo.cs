using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GoodsDiscountInfo
{

    public bool isDiscount = false;
    /***
	 * 价格打折比例（0-1）
	 */
    public float discountPercentage = 1;
    /***
	 * 打折后的价格
	 */
    //	public float discountPrice;
    /***
	 * 打折时间限制（格式：2019-01-12 00:00:00=2019-02-01 12:00:00）,不限制为null
	 */
    public String timeRange;

    public bool getDiscount()
    {
        return isDiscount;
    }
    public void setDiscount(bool isDiscount)
    {
        this.isDiscount = isDiscount;
    }
    public float getDiscountPercentage()
    {
        return discountPercentage;
    }
    public void setDiscountPercentage(float discountPercentage)
    {
        this.discountPercentage = discountPercentage;
    }
    public String getTimeRange()
    {
        return timeRange;
    }
    public void setTimeRange(String timeRange)
    {
        this.timeRange = timeRange;
    }
}
