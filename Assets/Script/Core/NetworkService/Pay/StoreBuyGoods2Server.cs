using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class StoreBuyGoods2Server
{
    /// <summary>
    /// 物品id
    /// </summary>
    public string id; 

    /// <summary>
    /// 数量
    /// </summary>
    public int number;
    public StoreName storeName;
    public string receipt;

    public StoreBuyGoods2Server()
    {
    }

    public StoreBuyGoods2Server(string id, int number, StoreName storeName, string receipt)
    {
        this.id = id;
        this.number = number;
        this.storeName = storeName;
        this.receipt = receipt;
    }

    public static void SenBuyMsg(string id, int number, StoreName storeName, string receipt)
    {
        JsonMessageProcessingController.SendMessage(new StoreBuyGoods2Server(id,number,storeName,receipt));
    }
}

