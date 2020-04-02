

public class PayResult
{
    public int code;
    public string goodsID;
    public string error;
    public StoreName storeName;

    public PayResult() { }

    public PayResult(int code, string goodsID, string error)
    {
        this.code = code;
        this.goodsID = goodsID;
        this.error = error;

    }
    public PayResult(int code, string goodsID, string error, StoreName storeName)
    {
        this.code = code;
        this.goodsID = goodsID;
        this.error = error;
        this.storeName = storeName;
    }
}